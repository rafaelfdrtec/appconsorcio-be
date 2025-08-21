using Microsoft.AspNetCore.Http;

namespace AppConsorciosMvp.Payments
{
    public interface IEscrowProvider
    {
        Task<EscrowIntentResult> CreateIntentAsync(Guid transacaoId, long amountCents, Split split, object? metadata);
        Task HandleWebhookAsync(HttpRequest request);
        Task<bool> ReleaseAsync(Guid transacaoId);
        Task<bool> RefundAsync(Guid transacaoId, string reason);
    }

    public record Split(long PlatformCents, string SellerAccountRef);
    public record EscrowIntentResult(string ProviderIntentId, string Status);
}
