using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using AppConsorciosMvp.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Controllers
{
    [ApiController]
    public class EscrowController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IEscrowProvider _provider;

        public EscrowController(AppDbContext db, IEscrowProvider provider)
        {
            _db = db;
            _provider = provider;
        }

        public record IntentBody(Guid transacaoId, long amount_centavos, Split split_json, object? metadata);

        [HttpPost("escrow/intent")]
        [Authorize]
        public async Task<ActionResult<object>> CreateIntent([FromBody] IntentBody body)
        {
            var tx = await _db.Transacoes.FindAsync(body.transacaoId);
            if (tx == null) return NotFound("Transação não encontrada");
            if (tx.Status != "contrato_assinado") return Conflict("409_STATE_TRANSITION_DENIED");

            var result = await _provider.CreateIntentAsync(body.transacaoId, body.amount_centavos, body.split_json, body.metadata);
            return Created(string.Empty, new { escrowId = result.ProviderIntentId, status = result.Status });
        }

        [HttpPost("webhooks/escrow")]
        [AllowAnonymous]
        public async Task<IActionResult> Webhook()
        {
            await _provider.HandleWebhookAsync(Request);
            return Ok();
        }

        public record ReleaseBody(Guid transacaoId);

        [HttpPost("escrow/release")]
        [Authorize]
        public async Task<ActionResult<object>> Release([FromBody] ReleaseBody body)
        {
            var tx = await _db.Transacoes.FindAsync(body.transacaoId);
            if (tx == null) return NotFound("Transação não encontrada");
            if (tx.Status != "transferencia_confirmada")
                return Conflict("409_BUSINESS_RULE_ESCROW_RELEASE");

            var ok = await _provider.ReleaseAsync(body.transacaoId);
            if (!ok) return NotFound("Escrow não encontrado");

            tx.Status = "escrow_liberado";
            tx.CurrentStep = "escrow_liberado";
            tx.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { status = "released" });
        }

        public record RefundBody(Guid transacaoId, string reason);

        [HttpPost("escrow/refund")]
        [Authorize]
        public async Task<ActionResult<object>> Refund([FromBody] RefundBody body)
        {
            var tx = await _db.Transacoes.FindAsync(body.transacaoId);
            if (tx == null) return NotFound("Transação não encontrada");

            var ok = await _provider.RefundAsync(body.transacaoId, body.reason);
            if (!ok) return NotFound("Escrow não encontrado");

            tx.Status = "cancelada";
            tx.CurrentStep = "cancelada";
            tx.UpdatedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();

            return Ok(new { status = "refunded" });
        }
    }
}
