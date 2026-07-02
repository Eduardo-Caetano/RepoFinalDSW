using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ObservatorioApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAtuacaoParlamentar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AtuacoesParlamentares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DeputadoId = table.Column<int>(type: "integer", nullable: false),
                    Propostas = table.Column<int>(type: "integer", nullable: false),
                    PropostasAutoria = table.Column<int>(type: "integer", nullable: false),
                    PropostasRelatadas = table.Column<int>(type: "integer", nullable: false),
                    Votacoes = table.Column<int>(type: "integer", nullable: false),
                    Discursos = table.Column<int>(type: "integer", nullable: false),
                    PresencasPlenario = table.Column<int>(type: "integer", nullable: false),
                    AusenciasJustificadasPlenario = table.Column<int>(type: "integer", nullable: false),
                    AusenciasNaoJustificadasPlenario = table.Column<int>(type: "integer", nullable: false),
                    PresencasComissoes = table.Column<int>(type: "integer", nullable: false),
                    AusenciasJustificadasComissoes = table.Column<int>(type: "integer", nullable: false),
                    AusenciasNaoJustificadasComissoes = table.Column<int>(type: "integer", nullable: false),
                    ComissoesTitular = table.Column<string>(type: "text", nullable: false),
                    ComissoesSuplente = table.Column<string>(type: "text", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtuacoesParlamentares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AtuacoesParlamentares_Deputados_DeputadoId",
                        column: x => x.DeputadoId,
                        principalTable: "Deputados",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AtuacoesParlamentares_DeputadoId",
                table: "AtuacoesParlamentares",
                column: "DeputadoId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AtuacoesParlamentares");
        }
    }
}
