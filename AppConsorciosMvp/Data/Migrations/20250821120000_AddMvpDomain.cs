using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppConsorciosMvp.Data.Migrations
{
    public partial class AddMvpDomain : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Usuario: novas colunas
            migrationBuilder.AddColumn<int>(
                name: "KycLevel",
                table: "Usuarios",
                type: "integer",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddColumn<bool>(
                name: "MfaEnabled",
                table: "Usuarios",
                type: "boolean",
                nullable: false,
                defaultValue: false);
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Usuarios",
                type: "varchar(30)",
                nullable: true);

            // kyc_cases
            migrationBuilder.CreateTable(
                name: "KycCases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    LevelRequested = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    EvidenceRefs = table.Column<string>(type: "jsonb", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: true),
                    ReasonCode = table.Column<string>(type: "varchar(50)", nullable: true),
                    ReasonMessage = table.Column<string>(type: "text", nullable: true),
                    Severity = table.Column<string>(type: "varchar(20)", nullable: false),
                    DueAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    BlocksJson = table.Column<string>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KycCases", x => x.Id);
                });
            migrationBuilder.CreateIndex(
                name: "IX_KycCases_UserId",
                table: "KycCases",
                column: "UserId");

            // transacoes
            migrationBuilder.CreateTable(
                name: "Transacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CartaId = table.Column<int>(type: "integer", nullable: false),
                    BuyerId = table.Column<int>(type: "integer", nullable: false),
                    SellerId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    CurrentStep = table.Column<string>(type: "varchar(50)", nullable: false),
                    StartedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transacoes", x => x.Id);
                });
            migrationBuilder.CreateIndex(name: "IX_Transacoes_CartaId", table: "Transacoes", column: "CartaId");
            migrationBuilder.CreateIndex(name: "IX_Transacoes_BuyerId", table: "Transacoes", column: "BuyerId");
            migrationBuilder.CreateIndex(name: "IX_Transacoes_SellerId", table: "Transacoes", column: "SellerId");

            // contratos
            migrationBuilder.CreateTable(
                name: "Contratos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: true),
                    EvidenceHash = table.Column<string>(type: "varchar(200)", nullable: false),
                    SignedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratos", x => x.Id);
                });
            migrationBuilder.CreateIndex(name: "IX_Contratos_TransacaoId", table: "Contratos", column: "TransacaoId");

            // escrow
            migrationBuilder.CreateTable(
                name: "Escrows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "varchar(50)", nullable: false),
                    IntentId = table.Column<string>(type: "varchar(100)", nullable: true),
                    AmountCentavos = table.Column<long>(type: "bigint", nullable: false),
                    FeeCentavos = table.Column<long>(type: "bigint", nullable: false),
                    SplitJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "varchar(30)", nullable: false),
                    ReleasedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    RefundedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escrows", x => x.Id);
                });
            migrationBuilder.CreateIndex(name: "IX_Escrows_TransacaoId", table: "Escrows", column: "TransacaoId");

            // chat_threads
            migrationBuilder.CreateTable(
                name: "ChatThreads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Kind = table.Column<string>(type: "varchar(30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatThreads", x => x.Id);
                });
            migrationBuilder.CreateIndex(name: "IX_ChatThreads_TransacaoId", table: "ChatThreads", column: "TransacaoId");

            // chat_messages
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorRole = table.Column<string>(type: "varchar(30)", nullable: false),
                    Type = table.Column<string>(type: "varchar(20)", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    AttachmentsJson = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                });
            migrationBuilder.CreateIndex(name: "IX_ChatMessages_ThreadId", table: "ChatMessages", column: "ThreadId");

            // notifications
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Channel = table.Column<string>(type: "varchar(30)", nullable: false),
                    Template = table.Column<string>(type: "varchar(50)", nullable: false),
                    PayloadJson = table.Column<string>(type: "jsonb", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    SentAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });
            migrationBuilder.CreateIndex(name: "IX_Notifications_UserId", table: "Notifications", column: "UserId");

            // audit_logs
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EntityType = table.Column<string>(type: "varchar(50)", nullable: false),
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Event = table.Column<string>(type: "varchar(50)", nullable: false),
                    PayloadHash = table.Column<string>(type: "varchar(200)", nullable: false),
                    PrevHash = table.Column<string>(type: "varchar(200)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActorId = table.Column<int>(type: "integer", nullable: false),
                    Ip = table.Column<string>(type: "varchar(60)", nullable: false),
                    Device = table.Column<string>(type: "varchar(120)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });
            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "AuditLogs");
            migrationBuilder.DropTable(name: "ChatMessages");
            migrationBuilder.DropTable(name: "ChatThreads");
            migrationBuilder.DropTable(name: "Contratos");
            migrationBuilder.DropTable(name: "Escrows");
            migrationBuilder.DropTable(name: "KycCases");
            migrationBuilder.DropTable(name: "Notifications");
            migrationBuilder.DropTable(name: "Transacoes");

            migrationBuilder.DropColumn(name: "KycLevel", table: "Usuarios");
            migrationBuilder.DropColumn(name: "MfaEnabled", table: "Usuarios");
            migrationBuilder.DropColumn(name: "Status", table: "Usuarios");
        }
    }
}
