using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    [Route("transacoes")]
    [Authorize]
    public class TransacoesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public TransacoesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<object>> Get([FromRoute] Guid id)
        {
            var tx = await _db.Transacoes.FindAsync(id);
            if (tx == null) return NotFound();

            var escrow = _db.Escrows.FirstOrDefault(e => e.TransacaoId == id);
            return Ok(new
            {
                id = tx.Id,
                status = tx.Status,
                currentStep = tx.CurrentStep,
                startedAt = tx.StartedAt,
                updatedAt = tx.UpdatedAt,
                totals = new
                {
                    escrow = escrow == null ? null : new
                    {
                        escrow.AmountCentavos,
                        escrow.FeeCentavos,
                        escrow.Status
                    }
                }
            });
        }

        [HttpPost("{id:guid}/cancelar")]
        public async Task<IActionResult> Cancel([FromRoute] Guid id)
        {
            var tx = await _db.Transacoes.FindAsync(id);
            if (tx == null) return NotFound();

            // regras simples para cancelamento
            if (tx.Status is "contrato_assinado" or "escrow_bloqueado")
            {
                tx.Status = "cancelada";
                tx.CurrentStep = "cancelada";
                tx.UpdatedAt = DateTimeOffset.UtcNow;
                await _db.SaveChangesAsync();
                return NoContent();
            }

            return Conflict("409_STATE_TRANSITION_DENIED");
        }
    }
}
