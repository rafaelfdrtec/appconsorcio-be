using AppConsorciosMvp.Data;
using AppConsorciosMvp.Models.Mvp;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AppConsorciosMvp.Payments
{
    public class MockEscrowProvider : IEscrowProvider
    {
        private readonly AppDbContext _db;
        private readonly ILogger<MockEscrowProvider> _logger;

        public MockEscrowProvider(AppDbContext db, ILogger<MockEscrowProvider> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<EscrowIntentResult> CreateIntentAsync(Guid transacaoId, long amountCents, Split split, object? metadata)
        {
            var escrow = await _db.Escrows.FirstOrDefaultAsync(e => e.TransacaoId == transacaoId);
            if (escrow == null)
            {
                escrow = new Escrow
                {
                    Id = Guid.NewGuid(),
                    TransacaoId = transacaoId,
                    Provider = "mock",
                    IntentId = $"intent_{Guid.NewGuid():N}",
                    AmountCentavos = amountCents,
                    FeeCentavos = split.PlatformCents,
                    SplitJson = System.Text.Json.JsonSerializer.Serialize(split),
                    Status = "intent_created"
                };
                _db.Escrows.Add(escrow);
                await _db.SaveChangesAsync();
            }
            return new EscrowIntentResult(escrow.IntentId!, "intent_created");
        }

        public async Task HandleWebhookAsync(HttpRequest request)
        {
            // Sandbox: autoriza sempre
            var intent = request.Query["intentId"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(intent)) return;

            var escrow = await _db.Escrows.FirstOrDefaultAsync(e => e.IntentId == intent);
            if (escrow == null) return;

            escrow.Status = "AUTHORIZED";
            await _db.SaveChangesAsync();

            // Atualiza transação para escrow_bloqueado
            var tx = await _db.Transacoes.FirstOrDefaultAsync(t => t.Id == escrow.TransacaoId);
            if (tx != null)
            {
                tx.Status = "escrow_bloqueado";
                tx.CurrentStep = "escrow_bloqueado";
                tx.UpdatedAt = DateTimeOffset.UtcNow;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ReleaseAsync(Guid transacaoId)
        {
            var escrow = await _db.Escrows.FirstOrDefaultAsync(e => e.TransacaoId == transacaoId);
            if (escrow == null) return false;
            escrow.Status = "RELEASED";
            escrow.ReleasedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RefundAsync(Guid transacaoId, string reason)
        {
            var escrow = await _db.Escrows.FirstOrDefaultAsync(e => e.TransacaoId == transacaoId);
            if (escrow == null) return false;
            escrow.Status = "REFUNDED";
            escrow.RefundedAt = DateTimeOffset.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
