using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace Telemetry.Infrastructure.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "telemetry_module");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "records",
                schema: "telemetry_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    invoker_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Details = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    occured_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    embedding = table.Column<Vector>(type: "vector(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_record_id", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_hnsw_record",
                schema: "telemetry_module",
                table: "records",
                column: "embedding")
                .Annotation("Npgsql:IndexMethod", "hnsw")
                .Annotation("Npgsql:IndexOperators", new[] { "vector_cosine_ops" });

            migrationBuilder.CreateIndex(
                name: "IX_records_invoker_id",
                schema: "telemetry_module",
                table: "records",
                column: "invoker_id");

            migrationBuilder.CreateIndex(
                name: "IX_records_occured_at",
                schema: "telemetry_module",
                table: "records",
                column: "occured_at",
                descending: new bool[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "records",
                schema: "telemetry_module");
        }
    }
}
