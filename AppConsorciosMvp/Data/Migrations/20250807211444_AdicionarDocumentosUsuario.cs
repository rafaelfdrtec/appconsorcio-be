using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppConsorciosMvp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarDocumentosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartasConsorcio_Usuarios_VendedorId",
                table: "CartasConsorcio");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataContemplacao",
                table: "CartasConsorcio",
                type: "timestamp",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Grupo",
                table: "CartasConsorcio",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NumeroCota",
                table: "CartasConsorcio",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "CartasConsorcio",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoContemplacao",
                table: "CartasConsorcio",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DocumentosUsuario",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    TipoDocumento = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    NomeArquivo = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    BlobUrl = table.Column<string>(type: "text", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    ContentType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "pendente"),
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

            migrationBuilder.AddForeignKey(
                name: "FK_CartasConsorcio_Usuarios_VendedorId",
                table: "CartasConsorcio",
                column: "VendedorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartasConsorcio_Usuarios_VendedorId",
                table: "CartasConsorcio");

            migrationBuilder.DropTable(
                name: "DocumentosUsuario");

            migrationBuilder.DropColumn(
                name: "DataContemplacao",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "Grupo",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "NumeroCota",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "TipoContemplacao",
                table: "CartasConsorcio");

            migrationBuilder.AddForeignKey(
                name: "FK_CartasConsorcio_Usuarios_VendedorId",
                table: "CartasConsorcio",
                column: "VendedorId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
