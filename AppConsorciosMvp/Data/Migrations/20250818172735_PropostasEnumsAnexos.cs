using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AppConsorciosMvp.Data.Migrations
{
    /// <inheritdoc />
    public partial class PropostasEnumsAnexos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompradorId",
                table: "CartasConsorcio",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataVenda",
                table: "CartasConsorcio",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropostaVendaId",
                table: "CartasConsorcio",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ValorVenda",
                table: "CartasConsorcio",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Arquivos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "CartaAnexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CartaConsorcioId = table.Column<int>(type: "integer", nullable: false),
                    ArquivoId = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_CartaAnexos_CartasConsorcio_CartaConsorcioId",
                        column: x => x.CartaConsorcioId,
                        principalTable: "CartasConsorcio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.ForeignKey(
                        name: "FK_Anexos_PropostasNegociacao_PropostaNegociacaoId",
                        column: x => x.PropostaNegociacaoId,
                        principalTable: "PropostasNegociacao",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropostaAnexos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PropostaNegociacaoId = table.Column<int>(type: "integer", nullable: false),
                    ArquivoId = table.Column<Guid>(type: "uuid", nullable: false),
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
                name: "IX_CartasConsorcio_CompradorId",
                table: "CartasConsorcio",
                column: "CompradorId");

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_PropostaVendaId",
                table: "CartasConsorcio",
                column: "PropostaVendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Anexos_PropostaNegociacaoId",
                table: "Anexos",
                column: "PropostaNegociacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CartaAnexos_ArquivoId",
                table: "CartaAnexos",
                column: "ArquivoId");

            migrationBuilder.CreateIndex(
                name: "IX_CartaAnexos_CartaConsorcioId",
                table: "CartaAnexos",
                column: "CartaConsorcioId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_CartasConsorcio_PropostasNegociacao_PropostaVendaId",
                table: "CartasConsorcio",
                column: "PropostaVendaId",
                principalTable: "PropostasNegociacao",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CartasConsorcio_Usuarios_CompradorId",
                table: "CartasConsorcio",
                column: "CompradorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartasConsorcio_PropostasNegociacao_PropostaVendaId",
                table: "CartasConsorcio");

            migrationBuilder.DropForeignKey(
                name: "FK_CartasConsorcio_Usuarios_CompradorId",
                table: "CartasConsorcio");

            migrationBuilder.DropTable(
                name: "Anexos");

            migrationBuilder.DropTable(
                name: "CartaAnexos");

            migrationBuilder.DropTable(
                name: "ParametroSistema");

            migrationBuilder.DropTable(
                name: "PropostaAnexos");

            migrationBuilder.DropTable(
                name: "Arquivos");

            migrationBuilder.DropTable(
                name: "PropostasNegociacao");

            migrationBuilder.DropIndex(
                name: "IX_CartasConsorcio_CompradorId",
                table: "CartasConsorcio");

            migrationBuilder.DropIndex(
                name: "IX_CartasConsorcio_PropostaVendaId",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "CompradorId",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "DataVenda",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "PropostaVendaId",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "ValorVenda",
                table: "CartasConsorcio");
        }
    }
}
