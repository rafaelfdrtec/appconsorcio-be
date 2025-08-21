using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppConsorciosMvp.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administradoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: false),
                    Telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Arquivos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeOriginal = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BlobUrl = table.Column<string>(type: "text", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arquivos", x => x.Id);
                });

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

            migrationBuilder.CreateTable(
                name: "ParametroSistema",
                columns: table => new
                {
                    Chave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametroSistema", x => x.Chave);
                });

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

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    Papel = table.Column<string>(type: "varchar(20)", nullable: false),
                    EhVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    KycLevel = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    MfaEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    Status = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "active")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosUsuario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumento = table.Column<string>(type: "varchar(50)", nullable: false),
                    NomeArquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    BlobUrl = table.Column<string>(type: "text", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false, defaultValue: "Pendente"),
                    ObservacoesValidacao = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AtualizadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ValidadoPorId = table.Column<int>(type: "integer", nullable: true),
                    ValidadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosUsuario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentosUsuario_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentosUsuario_Usuarios_ValidadoPorId",
                        column: x => x.ValidadoPorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropostaNegociacaoId = table.Column<int>(type: "integer", nullable: false),
                    NomeArquivo = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BlobUrl = table.Column<string>(type: "text", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anexos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartaAnexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartaConsorcioId = table.Column<int>(type: "integer", nullable: false),
                    ArquivoId = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaAnexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartaAnexos_Arquivos_ArquivoId",
                        column: x => x.ArquivoId,
                        principalTable: "Arquivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartasConsorcio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VendedorId = table.Column<int>(type: "integer", nullable: false),
                    AdministradoraId = table.Column<int>(type: "integer", nullable: false),
                    ValorCredito = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ValorEntrada = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ParcelasPagas = table.Column<int>(type: "integer", nullable: false),
                    ParcelasTotais = table.Column<int>(type: "integer", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    TipoBem = table.Column<string>(type: "varchar(50)", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    NumeroCota = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    Grupo = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    TipoContemplacao = table.Column<string>(type: "varchar(10)", nullable: false),
                    DataContemplacao = table.Column<DateTime>(type: "timestamp", nullable: true),
                    Observacoes = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EhVerificado = table.Column<bool>(type: "boolean", nullable: false),
                    CompradorId = table.Column<int>(type: "integer", nullable: true),
                    DataVenda = table.Column<DateTime>(type: "timestamp", nullable: true),
                    ValorVenda = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    PropostaVendaId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartasConsorcio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartasConsorcio_Administradoras_AdministradoraId",
                        column: x => x.AdministradoraId,
                        principalTable: "Administradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartasConsorcio_Usuarios_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CartasConsorcio_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropostasNegociacao",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartaConsorcioId = table.Column<int>(type: "integer", nullable: false),
                    CompradorId = table.Column<int>(type: "integer", nullable: false),
                    Agio = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PrazoMeses = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    MotivoCancelamento = table.Column<string>(type: "text", nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CanceladaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EfetivadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AnexoNomeArquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true),
                    AnexoContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true),
                    AnexoBlobUrl = table.Column<string>(type: "text", nullable: true),
                    AnexoBlobName = table.Column<string>(type: "text", nullable: true),
                    AnexoTamanhoBytes = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostasNegociacao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostasNegociacao_CartasConsorcio_CartaConsorcioId",
                        column: x => x.CartaConsorcioId,
                        principalTable: "CartasConsorcio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostasNegociacao_Usuarios_CompradorId",
                        column: x => x.CompradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PropostaAnexos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PropostaNegociacaoId = table.Column<int>(type: "integer", nullable: false),
                    ArquivoId = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropostaAnexos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropostaAnexos_Arquivos_ArquivoId",
                        column: x => x.ArquivoId,
                        principalTable: "Arquivos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropostaAnexos_PropostasNegociacao_PropostaNegociacaoId",
                        column: x => x.PropostaNegociacaoId,
                        principalTable: "PropostasNegociacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ParametroSistema",
                columns: new[] { "Chave", "Descricao", "Valor" },
                values: new object[,]
                {
                    { "Azure.Container.AnexosCartas", "Container para anexos de cartas", "anexos-cartas" },
                    { "Azure.Container.AnexosPropostas", "Container para anexos de propostas", "anexos-propostas" },
                    { "Azure.Container.Default", "Container padrão caso não definido", "documentos-usuarios" },
                    { "Azure.Container.DocumentosUsuarios", "Container para documentos de validação dos usuários (PII)", "documentos-usuarios" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administradoras_Cnpj",
                table: "Administradoras",
                column: "Cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_PropostaNegociacaoId",
                table: "Anexos",
                column: "PropostaNegociacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_CartaAnexos_ArquivoId",
                table: "CartaAnexos",
                column: "ArquivoId");

            migrationBuilder.CreateIndex(
                name: "IX_CartaAnexos_CartaConsorcioId",
                table: "CartaAnexos",
                column: "CartaConsorcioId");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_AdministradoraId",
                table: "CartasConsorcio",
                column: "AdministradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_CompradorId",
                table: "CartasConsorcio",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_PropostaVendaId",
                table: "CartasConsorcio",
                column: "PropostaVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_Status",
                table: "CartasConsorcio",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_TipoBem",
                table: "CartasConsorcio",
                column: "TipoBem");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_VendedorId",
                table: "CartasConsorcio",
                column: "VendedorId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ThreadId",
                table: "ChatMessages",
                column: "ThreadId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatThreads_TransacaoId",
                table: "ChatThreads",
                column: "TransacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_Contratos_TransacaoId",
                table: "Contratos",
                column: "TransacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosUsuario_Status",
                table: "DocumentosUsuario",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosUsuario_TipoDocumento",
                table: "DocumentosUsuario",
                column: "TipoDocumento");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosUsuario_UsuarioId",
                table: "DocumentosUsuario",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosUsuario_UsuarioId_TipoDocumento",
                table: "DocumentosUsuario",
                columns: new[] { "UsuarioId", "TipoDocumento" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosUsuario_ValidadoPorId",
                table: "DocumentosUsuario",
                column: "ValidadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Escrows_TransacaoId",
                table: "Escrows",
                column: "TransacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_KycCases_UserId",
                table: "KycCases",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostaAnexos_ArquivoId",
                table: "PropostaAnexos",
                column: "ArquivoId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostaAnexos_PropostaNegociacaoId",
                table: "PropostaAnexos",
                column: "PropostaNegociacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasNegociacao_CartaConsorcioId",
                table: "PropostasNegociacao",
                column: "CartaConsorcioId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasNegociacao_CompradorId",
                table: "PropostasNegociacao",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_PropostasNegociacao_Status",
                table: "PropostasNegociacao",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_BuyerId",
                table: "Transacoes",
                column: "BuyerId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_CartaId",
                table: "Transacoes",
                column: "CartaId");

            migrationBuilder.CreateIndex(
                name: "IX_Transacoes_SellerId",
                table: "Transacoes",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Anexos_PropostasNegociacao_PropostaNegociacaoId",
                table: "Anexos",
                column: "PropostaNegociacaoId",
                principalTable: "PropostasNegociacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartaAnexos_CartasConsorcio_CartaConsorcioId",
                table: "CartaAnexos",
                column: "CartaConsorcioId",
                principalTable: "CartasConsorcio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CartasConsorcio_PropostasNegociacao_PropostaVendaId",
                table: "CartasConsorcio",
                column: "PropostaVendaId",
                principalTable: "PropostasNegociacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartasConsorcio_PropostasNegociacao_PropostaVendaId",
                table: "CartasConsorcio");

            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "CartaAnexos");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatThreads");

            migrationBuilder.DropTable(
                name: "Contratos");

            migrationBuilder.DropTable(
                name: "DocumentosUsuario");

            migrationBuilder.DropTable(
                name: "Escrows");

            migrationBuilder.DropTable(
                name: "KycCases");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "ParametroSistema");

            migrationBuilder.DropTable(
                name: "PropostaAnexos");

            migrationBuilder.DropTable(
                name: "Transacoes");

            migrationBuilder.DropTable(
                name: "Arquivos");

            migrationBuilder.DropTable(
                name: "PropostasNegociacao");

            migrationBuilder.DropTable(
                name: "CartasConsorcio");

            migrationBuilder.DropTable(
                name: "Administradoras");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
