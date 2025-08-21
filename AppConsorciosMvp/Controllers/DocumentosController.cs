using System.Security.Claims;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Models;
using AppConsorciosMvp.Models.Enums;
using AppConsorciosMvp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requer autenticação para todos os endpoints
    public class DocumentosController(
        AppDbContext context,
        AzureBlobService blobService,
        ILogger<DocumentosController> logger)
        : ControllerBase
    {
        // Tipos de arquivo permitidos
        private readonly string[] _tiposPermitidos = { ".pdf", ".jpg", ".jpeg", ".png" };
        private readonly string[] _contentTypesPermitidos = 
        { 
            "application/pdf", 
            "image/jpeg", 
            "image/jpg", 
            "image/png" 
        };
        private const long TamanhoMaximo = 10 * 1024 * 1024; // 10MB

        /// <summary>
        /// Upload de documento do usuário
        /// </summary>
        [HttpPost("upload")]
        public async Task<ActionResult<DocumentoUsuarioRespostaDTO>> UploadDocumento([FromForm] UploadDocumentoDTO dto)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var usuario = await context.Usuarios.FindAsync(usuarioId);

                if (usuario == null)
                {
                    return BadRequest("Usuário não encontrado");
                }

                // Validar arquivo
                var validacao = ValidarArquivo(dto.Arquivo);
                if (!string.IsNullOrEmpty(validacao))
                {
                    return BadRequest(validacao);
                }

                // Normaliza o tipo do documento
                var tipo = ParseDocumentoTipo(dto.TipoDocumento);

                // Verificar se já existe documento do mesmo tipo para este usuário
                var documentoExistente = await context.DocumentosUsuario
                    .FirstOrDefaultAsync(d => d.UsuarioId == usuarioId && d.TipoDocumento == tipo);

                if (documentoExistente != null)
                {
                    // Excluir o blob antigo
                    await blobService.DeleteAsync(AzureBlobService.ParamContainerDocumentosUsuarios, documentoExistente.BlobName);
                    context.DocumentosUsuario.Remove(documentoExistente);
                }

                // Fazer upload para Azure Blob Storage
                using var stream = dto.Arquivo.OpenReadStream();
                var (blobName, blobUrl) = await blobService.UploadAsync(
                    stream, 
                    dto.Arquivo.FileName, 
                    dto.Arquivo.ContentType, 
                    usuarioId, 
                    dto.TipoDocumento);

                // Salvar metadados no banco
                var documento = new DocumentoUsuario
                {
                    UsuarioId = usuarioId,
                    TipoDocumento = tipo,
                    NomeArquivo = dto.Arquivo.FileName,
                    BlobUrl = blobUrl,
                    BlobName = blobName,
                    ContentType = dto.Arquivo.ContentType,
                    TamanhoBytes = dto.Arquivo.Length,
                    Status = DocumentoStatus.Pendente,
                    CriadoEm = DateTime.UtcNow,
                    AtualizadoEm = DateTime.UtcNow
                };

                context.DocumentosUsuario.Add(documento);
                await context.SaveChangesAsync();

                logger.LogInformation($"Documento {dto.TipoDocumento} enviado pelo usuário {usuarioId}");

                return CreatedAtAction(nameof(ObterDocumento), new { id = documento.Id }, 
                    MapearParaDto(documento, usuario.Nome));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao fazer upload de documento");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Obter documentos do usuário logado
        /// </summary>
        [HttpGet("meus-documentos")]
        public async Task<ActionResult<IEnumerable<DocumentoUsuarioRespostaDTO>>> ObterMeusDocumentos()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var documentos = await context.DocumentosUsuario
                .Where(d => d.UsuarioId == usuarioId)
                .Include(d => d.Usuario)
                .Include(d => d.ValidadoPor)
                .OrderByDescending(d => d.CriadoEm)
                .ToListAsync();

            var result = documentos.Select(d => MapearParaDto(d, d.Usuario?.Nome ?? "", d.ValidadoPor?.Nome));
            return Ok(result);
        }

        /// <summary>
        /// Obter documento específico (apenas admin ou dono)
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DocumentoUsuarioRespostaDTO>> ObterDocumento(int id)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var papel = User.FindFirst(ClaimTypes.Role)?.Value;

            var documento = await context.DocumentosUsuario
                .Include(d => d.Usuario)
                .Include(d => d.ValidadoPor)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado");
            }

            // Verificar permissão
            if (documento.UsuarioId != usuarioId && papel != "admin")
            {
                return Forbid("Acesso negado");
            }

            return Ok(MapearParaDto(documento, documento.Usuario?.Nome ?? "", documento.ValidadoPor?.Nome));
        }

        /// <summary>
        /// Download de documento (apenas admin ou dono)
        /// </summary>
        [HttpGet("{id:int}/download")]
        public async Task<ActionResult> DownloadDocumento(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var papel = User.FindFirst(ClaimTypes.Role)?.Value;

                var documento = await context.DocumentosUsuario.FindAsync(id);

                if (documento == null)
                {
                    return NotFound("Documento não encontrado");
                }

                // Verificar permissão
                if (documento.UsuarioId != usuarioId && papel != "admin")
                {
                    return Forbid("Acesso negado");
                }

                var (stream, contentType) = await blobService.DownloadAsync(AzureBlobService.ParamContainerDocumentosUsuarios, documento.BlobName);

                return File(stream, contentType, documento.NomeArquivo);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Arquivo não encontrado no storage");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao fazer download do documento {id}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        /// <summary>
        /// Listar todos os documentos (apenas admin)
        /// </summary>
        [HttpGet("admin/todos")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<DocumentoUsuarioRespostaDTO>>> ListarTodosDocumentos(
            [FromQuery] string? status = null,
            [FromQuery] string? tipoDocumento = null)
        {
            IQueryable<DocumentoUsuario> query = context.DocumentosUsuario
                .Include(d => d.Usuario)
                .Include(d => d.ValidadoPor);

            if (!string.IsNullOrEmpty(status))
            {
                var st = ParseDocumentoStatus(status);
                query = query.Where(d => d.Status == st);
            }

            if (!string.IsNullOrEmpty(tipoDocumento))
            {
                var tp = ParseDocumentoTipo(tipoDocumento);
                query = query.Where(d => d.TipoDocumento == tp);
            }

            var documentos = await query
                .OrderByDescending(d => d.CriadoEm)
                .ToListAsync();

            var result = documentos.Select(d => MapearParaDto(d, d.Usuario?.Nome ?? "", d.ValidadoPor?.Nome));
            return Ok(result);
        }

        /// <summary>
        /// Validar documento (apenas admin)
        /// </summary>
        [HttpPut("{id:int}/validar")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<DocumentoUsuarioRespostaDTO>> ValidarDocumento(int id, ValidarDocumentoDTO dto)
        {
            var validadorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var documento = await context.DocumentosUsuario
                .Include(d => d.Usuario)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado");
            }

            documento.Status = ParseDocumentoStatus(dto.Status);
            documento.ObservacoesValidacao = dto.ObservacoesValidacao;
            documento.ValidadoPorId = validadorId;
            documento.ValidadoEm = DateTime.UtcNow;
            documento.AtualizadoEm = DateTime.UtcNow;

            await context.SaveChangesAsync();

            logger.LogInformation($"Documento {id} validado como {dto.Status} por admin {validadorId}");

            var validador = await context.Usuarios.FindAsync(validadorId);
            return Ok(MapearParaDto(documento, documento.Usuario?.Nome ?? "", validador?.Nome));
        }

        /// <summary>
        /// Excluir documento
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> ExcluirDocumento(int id)
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var papel = User.FindFirst(ClaimTypes.Role)?.Value;

                var documento = await context.DocumentosUsuario.FindAsync(id);

                if (documento == null)
                {
                    return NotFound("Documento não encontrado");
                }

                // Verificar permissão
                if (documento.UsuarioId != usuarioId && papel != "admin")
                {
                    return Forbid("Acesso negado");
                }

                // Excluir do Azure Blob Storage
                await blobService.DeleteAsync(AzureBlobService.ParamContainerDocumentosUsuarios, documento.BlobName);

                // Excluir do banco
                context.DocumentosUsuario.Remove(documento);
                await context.SaveChangesAsync();

                logger.LogInformation($"Documento {id} excluído");

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Erro ao excluir documento {id}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        private string ValidarArquivo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return "Arquivo é obrigatório";
            }

            if (arquivo.Length > TamanhoMaximo)
            {
                return $"Arquivo muito grande. Máximo permitido: {TamanhoMaximo / 1024 / 1024}MB";
            }

            var extension = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!_tiposPermitidos.Contains(extension))
            {
                return $"Tipo de arquivo não permitido. Permitidos: {string.Join(", ", _tiposPermitidos)}";
            }

            if (!_contentTypesPermitidos.Contains(arquivo.ContentType))
            {
                return $"Tipo de conteúdo não permitido. Permitidos: {string.Join(", ", _contentTypesPermitidos)}";
            }

            return string.Empty;
        }

        private DocumentoUsuarioRespostaDTO MapearParaDto(DocumentoUsuario documento, string nomeUsuario, string? nomeValidador = null)
        {
            return new DocumentoUsuarioRespostaDTO
            {
                Id = documento.Id.ToString(),
                UsuarioId = documento.UsuarioId,
                NomeUsuario = nomeUsuario,
                TipoDocumento = documento.TipoDocumento.ToString(),
                NomeArquivo = documento.NomeArquivo,
                ContentType = documento.ContentType,
                TamanhoBytes = documento.TamanhoBytes,
                Status = documento.Status.ToString().ToLower(),
                ObservacoesValidacao = documento.ObservacoesValidacao,
                CriadoEm = documento.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss"),
                AtualizadoEm = documento.AtualizadoEm.ToString("yyyy-MM-dd HH:mm:ss"),
                ValidadoPorId = documento.ValidadoPorId,
                NomeValidador = nomeValidador,
                ValidadoEm = documento.ValidadoEm?.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        private static DocumentoTipo ParseDocumentoTipo(string tipo) =>
            (tipo ?? "").Trim().ToLower() switch
            {
                "rg" => DocumentoTipo.RG,
                "cpf" => DocumentoTipo.CPF,
                "cnh" => DocumentoTipo.CNH,
                "comprovanteresidencia" => DocumentoTipo.ComprovanteResidencia,
                "comprovanteResidencia" => DocumentoTipo.ComprovanteResidencia,
                _ => DocumentoTipo.Outro
            };

        private static DocumentoStatus ParseDocumentoStatus(string status) =>
            (status ?? "").Trim().ToLower() switch
            {
                "aprovado" => DocumentoStatus.Aprovado,
                "rejeitado" => DocumentoStatus.Rejeitado,
                "pendente" => DocumentoStatus.Pendente,
                _ => DocumentoStatus.Pendente
            };
    }
}
