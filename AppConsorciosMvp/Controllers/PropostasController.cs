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
    [Authorize]
    public class PropostasController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AzureBlobService _blob;
        private readonly ILogger<PropostasController> _logger;

        private readonly string[] _extPermitidas = { ".pdf", ".jpg", ".jpeg", ".png" };
        private readonly string[] _ctPermitidos = { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
        private const long _tamanhoMaximo = 10 * 1024 * 1024; // 10MB

        public PropostasController(AppDbContext db, AzureBlobService blob, ILogger<PropostasController> logger)
        {
            _db = db;
            _blob = blob;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<PropostaRespostaDTO>> Criar([FromForm] CriarPropostaDTO dto)
        {
            var compradorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var carta = await _db.CartasConsorcio.FirstOrDefaultAsync(c => c.Id == dto.CartaConsorcioId);
            if (carta == null) return NotFound("Carta não encontrada");
            if (carta.Status == CartaStatus.Vendida) return BadRequest("Carta já vendida");

            var proposta = new PropostaNegociacao
            {
                CartaConsorcioId = carta.Id,
                CompradorId = compradorId,
                Agio = dto.Agio,
                PrazoMeses = dto.PrazoMeses,
                Status = PropostaStatus.Iniciada,
                CriadaEm = DateTime.UtcNow
            };

            _db.PropostasNegociacao.Add(proposta);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorCarta), new { cartaId = carta.Id }, Mapear(proposta, null));
        }

        [HttpGet("carta/{cartaId:int}")]
        public async Task<ActionResult<IEnumerable<PropostaRespostaDTO>>> ObterPorCarta(int cartaId)
        {
            var propostas = await _db.PropostasNegociacao
                .Where(p => p.CartaConsorcioId == cartaId)
                .Include(p => p.Comprador)
                .OrderByDescending(p => p.CriadaEm)
                .ToListAsync();

            return Ok(propostas.Select(p => Mapear(p, p.Comprador?.Nome)));
        }

        [HttpPost("{id:int}/cancelar")]
        public async Task<ActionResult<PropostaRespostaDTO>> Cancelar(int id, [FromBody] CancelarPropostaDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var proposta = await _db.PropostasNegociacao
                .Include(p => p.Carta)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proposta == null) return NotFound("Proposta não encontrada");
            if (proposta.Status == PropostaStatus.Efetivada) return BadRequest("Proposta já efetivada");

            var carta = proposta.Carta!;
            var vendedorPode = carta.VendedorId == usuarioId;
            var compradorPode = proposta.CompradorId == usuarioId;
            var adminPode = role == "admin";
            if (!(vendedorPode || compradorPode || adminPode))
                return Forbid();

            proposta.Status = PropostaStatus.Cancelada;
            proposta.MotivoCancelamento = dto.Motivo;
            proposta.CanceladaEm = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return Ok(Mapear(proposta, null));
        }

        [HttpPost("{id:int}/efetivar")]
        public async Task<ActionResult<PropostaRespostaDTO>> Efetivar(int id, [FromBody] EfetivarPropostaDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var proposta = await _db.PropostasNegociacao
                .Include(p => p.Carta)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proposta == null) return NotFound("Proposta não encontrada");
            if (proposta.Status == PropostaStatus.Cancelada) return BadRequest("Proposta cancelada");
            if (proposta.Status == PropostaStatus.Efetivada) return BadRequest("Proposta já efetivada");

            var carta = proposta.Carta!;
            if (carta.Status == CartaStatus.Vendida) return BadRequest("Carta já vendida");

            if (!(carta.VendedorId == usuarioId || role == "admin"))
                return Forbid();

            // Efetivar
            proposta.Status = PropostaStatus.Efetivada;
            proposta.EfetivadaEm = DateTime.UtcNow;

            carta.Status = CartaStatus.Vendida;
            carta.DataVenda = DateTime.UtcNow;
            carta.ValorVenda = dto.ValorVenda;
            carta.CompradorId = proposta.CompradorId;
            carta.PropostaVendaId = proposta.Id;

            await _db.SaveChangesAsync();
            return Ok(Mapear(proposta, null));
        }

        [HttpPost("{id:int}/anexos")]
        public async Task<ActionResult<AnexoRespostaDTO>> UploadAnexo(int id, [FromForm] UploadAnexoPropostaDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var proposta = await _db.PropostasNegociacao
                .Include(p => p.Carta)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proposta == null) return NotFound("Proposta não encontrada");

            // Permissão: comprador, vendedor da carta ou admin
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var carta = proposta.Carta!;
            if (!(proposta.CompradorId == usuarioId || carta.VendedorId == usuarioId || role == "admin"))
                return Forbid();

            var arq = dto.Arquivo;
            var erro = ValidarArquivo(arq);
            if (!string.IsNullOrEmpty(erro)) return BadRequest(erro);

            using var stream = arq.OpenReadStream();
            var (blobName, blobUrl) = await _blob.UploadAnexoPropostaAsync(stream, arq.FileName, arq.ContentType, proposta.Id);

            var arquivo = new Arquivo
            {
                NomeOriginal = arq.FileName,
                ContentType = arq.ContentType,
                TamanhoBytes = arq.Length,
                BlobName = blobName,
                BlobUrl = blobUrl,
                CriadoEm = DateTime.UtcNow
            };
            _db.Arquivos.Add(arquivo);

            var vinculo = new PropostaAnexo
            {
                PropostaNegociacaoId = proposta.Id,
                ArquivoId = arquivo.Id,
                CriadoEm = DateTime.UtcNow
            };
            _db.PropostaAnexos.Add(vinculo);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorCarta), new { cartaId = proposta.CartaConsorcioId }, new AnexoRespostaDTO
            {
                Id = arquivo.Id.ToString(),
                NomeOriginal = arquivo.NomeOriginal,
                ContentType = arquivo.ContentType,
                TamanhoBytes = arquivo.TamanhoBytes,
                BlobName = arquivo.BlobName,
                BlobUrl = arquivo.BlobUrl,
                CriadoEm = arquivo.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }

        private string ValidarArquivo(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0) return "Arquivo é obrigatório";
            if (arquivo.Length > _tamanhoMaximo) return $"Arquivo muito grande. Máximo permitido: {_tamanhoMaximo / 1024 / 1024}MB";
            var ext = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            if (!_extPermitidas.Contains(ext)) return $"Extensão não permitida. Permitidos: {string.Join(", ", _extPermitidas)}";
            if (!_ctPermitidos.Contains(arquivo.ContentType)) return $"Content-Type não permitido. Permitidos: {string.Join(", ", _ctPermitidos)}";
            return string.Empty;
        }

        private static PropostaRespostaDTO Mapear(PropostaNegociacao p, string? nomeComprador)
        {
            return new PropostaRespostaDTO
            {
                Id = p.Id,
                CartaConsorcioId = p.CartaConsorcioId,
                CompradorId = p.CompradorId,
                NomeComprador = nomeComprador,
                Agio = p.Agio,
                PrazoMeses = p.PrazoMeses,
                Status = p.Status.ToString(),
                MotivoCancelamento = p.MotivoCancelamento,
                CriadaEm = p.CriadaEm.ToString("yyyy-MM-dd HH:mm:ss"),
                CanceladaEm = p.CanceladaEm?.ToString("yyyy-MM-dd HH:mm:ss"),
                EfetivadaEm = p.EfetivadaEm?.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
