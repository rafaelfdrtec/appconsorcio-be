using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppConsorciosMvp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAdministradoraEForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AdministradoraId",
                table: "CartasConsorcio",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Administradoras",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    Cnpj = table.Column<string>(type: "varchar(18)", maxLength: 18, nullable: false),
                    Telefone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true),
                    Status = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, defaultValue: "ativa"),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administradoras", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartasConsorcio_AdministradoraId",
                table: "CartasConsorcio",
                column: "AdministradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_Administradoras_Cnpj",
                table: "Administradoras",
                column: "Cnpj",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CartasConsorcio_Administradoras_AdministradoraId",
                table: "CartasConsorcio",
                column: "AdministradoraId",
                principalTable: "Administradoras",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartasConsorcio_Administradoras_AdministradoraId",
                table: "CartasConsorcio");

            migrationBuilder.DropTable(
                name: "Administradoras");

            migrationBuilder.DropIndex(
                name: "IX_CartasConsorcio_AdministradoraId",
                table: "CartasConsorcio");

            migrationBuilder.DropColumn(
                name: "AdministradoraId",
                table: "CartasConsorcio");
        }
    }
}
