using System.Security.Claims;
using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("contratos")]
    [Authorize]
    public class ContratosController(AppDbContext db, ILogger<ContratosController> logger) : ControllerBase
    {
        private readonly ILogger<ContratosController> _logger = logger;

        public record UploadBody(Guid id, string url, string EvidenceHash);

        [HttpPost("{id:guid}/upload")]
        public async Task<IActionResult> Upload([FromRoute] Guid id, [FromBody] UploadBody body)
        {
            var contrato = await db.Contratos.FindAsync(id);
            if (contrato == null)
            {
                // cria contrato se não existir
                contrato = new Contrato
                {
                    Id = id,
                    TransacaoId = id, // fallback: em fluxo real viria do create
                    Status = "assinado",
                    Url = body.url,
                    EvidenceHash = body.EvidenceHash,
                    SignedAt = DateTimeOffset.UtcNow
                };
                db.Contratos.Add(contrato);
            }
            else
            {
                contrato.Url = body.url;
                contrato.EvidenceHash = body.EvidenceHash;
                contrato.SignedAt = DateTimeOffset.UtcNow;
                contrato.Status = "assinado";
            }

            await db.SaveChangesAsync();

            // Atualiza transação para contrato_assinado
            var tx = await db.Transacoes.FirstOrDefaultAsync(t => t.Id == contrato.TransacaoId);
            if (tx != null)
            {
                tx.Status = "contrato_assinado";
                tx.CurrentStep = "contrato_assinado";
                tx.UpdatedAt = DateTimeOffset.UtcNow;
                await db.SaveChangesAsync();
            }

            return NoContent();
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<object>> Get([FromRoute] Guid id)
        {
            var contrato = await db.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();

            return Ok(new
            {
                id = contrato.Id,
                status = contrato.Status,
                url = contrato.Url,
                signedAt = contrato.SignedAt
            });
        }
    }
}
