using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CsharpORM.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class MigrationDbEmprestimo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cpf = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Emprestimos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Valor = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Parcelas = table.Column<int>(type: "int", nullable: false),
                    TaxaJuros = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Emprestimos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Emprestimos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Cpf", "Nome" },
                values: new object[,]
                {
                    { 1, "123.456.789-00", "João da Silva" },
                    { 2, "987.654.321-00", "Maria Oliveira" }
                });

            migrationBuilder.InsertData(
                table: "Emprestimos",
                columns: new[] { "Id", "ClienteId", "Parcelas", "TaxaJuros", "Valor" },
                values: new object[,]
                {
                    { 1, 1, 12, 2.5m, 1000.00m },
                    { 2, 1, 24, 3.0m, 5000.00m },
                    { 3, 2, 10, 1.8m, 2000.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Emprestimos_ClienteId",
                table: "Emprestimos",
                column: "ClienteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Emprestimos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
