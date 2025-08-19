using System.Security.Claims;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.DTOs;
using AppConsorciosMvp.Models;
using AppConsorciosMvp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("api/cartas/{cartaId:int}/anexos")]
    [Authorize]
    public class CartaAnexosController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly AzureBlobService _blob;

        private readonly string[] _extPermitidas = { ".pdf", ".jpg", ".jpeg", ".png" };
        private readonly string[] _ctPermitidos = { "application/pdf", "image/jpeg", "image/jpg", "image/png" };
        private const long _tamanhoMaximo = 10 * 1024 * 1024;

        public CartaAnexosController(AppDbContext db, AzureBlobService blob)
        {
            _db = db;
            _blob = blob;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnexoRespostaDTO>>> Listar(int cartaId)
        {
            var anexos = await _db.CartaAnexos
                .Where(ca => ca.CartaConsorcioId == cartaId)
                .Include(ca => ca.Arquivo)
                .OrderByDescending(ca => ca.CriadoEm)
                .ToListAsync();

            return Ok(anexos.Select(a => new AnexoRespostaDTO
            {
                Id = a.ArquivoId.ToString(),
                NomeOriginal = a.Arquivo!.NomeOriginal,
                ContentType = a.Arquivo.ContentType,
                TamanhoBytes = a.Arquivo.TamanhoBytes,
                BlobName = a.Arquivo.BlobName,
                BlobUrl = a.Arquivo.BlobUrl,
                CriadoEm = a.Arquivo.CriadoEm.ToString("yyyy-MM-dd HH:mm:ss")
            }));
        }

        [HttpPost]
        public async Task<ActionResult<AnexoRespostaDTO>> Upload(int cartaId, [FromForm] UploadAnexoCartaDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var role = User.FindFirst(ClaimTypes.Role)?.Value;

            var carta = await _db.CartasConsorcio.FindAsync(cartaId);
            if (carta == null) return NotFound("Carta não encontrada");

            // Permissão: vendedor da carta ou admin
            if (!(carta.VendedorId == usuarioId || role == "admin"))
                return Forbid();

            var arq = dto.Arquivo;
            var erro = ValidarArquivo(arq);
            if (!string.IsNullOrEmpty(erro)) return BadRequest(erro);

            using var stream = arq.OpenReadStream();
            var (blobName, blobUrl) = await _blob.UploadAnexoCartaAsync(stream, arq.FileName, arq.ContentType, carta.Id);

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

            var vinculo = new CartaAnexo
            {
                CartaConsorcioId = carta.Id,
                ArquivoId = arquivo.Id,
                CriadoEm = DateTime.UtcNow
            };
            _db.CartaAnexos.Add(vinculo);

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Listar), new { cartaId = carta.Id }, new AnexoRespostaDTO
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
    }
}
