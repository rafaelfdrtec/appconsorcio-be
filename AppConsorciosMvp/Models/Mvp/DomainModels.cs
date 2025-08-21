using System.ComponentModel.DataAnnotations;

namespace AppConsorciosMvp.Models.Mvp
{
    public record KycCase
    {
        [Key] public Guid Id { get; init; }
        public int UserId { get; init; }
        public int LevelRequested { get; init; }
        public string Status { get; set; } = "pending";
        public string EvidenceRefs { get; set; } = "[]";
        public int? Score { get; set; }
        public string? ReasonCode { get; set; }
        public string? ReasonMessage { get; set; }
        public string Severity { get; set; } = "informative";
        public DateTimeOffset? DueAt { get; set; }
        public string BlocksJson { get; set; } = "[]";
    }

    public record Transacao
    {
        [Key] public Guid Id { get; init; }
        public int CartaId { get; init; }
        public int BuyerId { get; init; }
        public int SellerId { get; init; }
        public string Status { get; set; } = "proposta_aceita";
        public string CurrentStep { get; set; } = "proposta_aceita";
        public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }

    public record Contrato
    {
        [Key] public Guid Id { get; init; }
        public Guid TransacaoId { get; init; }
        public string Status { get; set; } = "pendente";
        public string? Url { get; set; }
        public string EvidenceHash { get; set; } = string.Empty;
        public DateTimeOffset? SignedAt { get; set; }
    }

    public record Escrow
    {
        [Key] public Guid Id { get; init; }
        public Guid TransacaoId { get; init; }
        public string Provider { get; init; } = "mock";
        public string? IntentId { get; set; }
        public long AmountCentavos { get; init; }
        public long FeeCentavos { get; init; }
        public string SplitJson { get; init; } = "{}";
        public string Status { get; set; } = "created";
        public DateTimeOffset? ReleasedAt { get; set; }
        public DateTimeOffset? RefundedAt { get; set; }
    }

    public record ChatThread
    {
        [Key] public Guid Id { get; init; }
        public Guid TransacaoId { get; init; }
        public string Kind { get; init; } = "buyer_agent"; // buyer_agent | seller_agent
    }

    public record ChatMessage
    {
        [Key] public Guid Id { get; init; }
        public Guid ThreadId { get; init; }
        public string AuthorRole { get; init; } = "buyer_agent";
        public string Type { get; init; } = "normal";
        public string Text { get; init; } = string.Empty;
        public string AttachmentsJson { get; init; } = "[]";
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    }

    public record Notification
    {
        [Key] public Guid Id { get; init; }
        public int UserId { get; init; }
        public string Channel { get; init; } = "email";
        public string Template { get; init; } = string.Empty;
        public string PayloadJson { get; init; } = "{}";
        public string Status { get; set; } = "queued";
        public DateTimeOffset? SentAt { get; set; }
    }

    public record AuditLog
    {
        [Key] public Guid Id { get; init; }
        public string EntityType { get; init; } = string.Empty;
        public Guid EntityId { get; init; }
        public string Event { get; init; } = string.Empty;
        public string PayloadHash { get; init; } = string.Empty;
        public string PrevHash { get; init; } = string.Empty;
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public int ActorId { get; init; }
        public string Ip { get; init; } = string.Empty;
        public string Device { get; init; } = string.Empty;
    }
}
