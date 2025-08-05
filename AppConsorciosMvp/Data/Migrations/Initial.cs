using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppConsorciosMvp.Data.Migrations
{
    /// <summary>
    /// Migração inicial do banco de dados
    /// </summary>
    public partial class Initial : Migration
    {
        /// <summary>
        /// Método para criar as tabelas iniciais
        /// </summary>
        /// <param name="migrationBuilder">Builder de migrações</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Papel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EhVerificado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CartasConsorcio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendedorId = table.Column<int>(type: "int", nullable: false),
                    ValorCredito = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValorEntrada = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ParcelasPagas = table.Column<int>(type: "int", nullable: false),
                    ParcelasTotais = table.Column<int>(type: "int", nullable: false),
                    ValorParcela = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TipoBem = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EhVerificado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartasConsorcio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartasConsorcio_Usuarios_VendedorId",
                        column: x => x.VendedorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            // Criar um usuário administrador para início do sistema
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Nome", "Email", "SenhaHash", "Papel", "EhVerificado" },
                values: new object[] {
                    "Administrador", 
                    "admin@consorcios.com", 
                    // Hash da senha "Admin@123"
                    "FqfJRIVigSXmUoV+CRljNr9yN4v9FRXPfCDTQ0R+qCc=:MvNsQUgnA7CxXRgPFvRqYEFN7mM9UYvnUc6DzPkUhQE=", 
                    "admin", 
                    true 
                });
        }

        /// <summary>
        /// Método para remover as tabelas
        /// </summary>
        /// <param name="migrationBuilder">Builder de migrações</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartasConsorcio");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
