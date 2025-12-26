using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cleaners.Adapter.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Cleaners_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cleaners_module");

            migrationBuilder.CreateTable(
                name: "cleaners",
                schema: "cleaners_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cleaned_amount = table.Column<int>(type: "integer", nullable: false),
                    last_run = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    next_run = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    wait_days = table.Column<int>(type: "integer", nullable: false),
                    state = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    hours = table.Column<int>(type: "integer", nullable: false),
                    minutes = table.Column<int>(type: "integer", nullable: false),
                    seconds = table.Column<int>(type: "integer", nullable: false),
                    items_date_day_threshold = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cleaners", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox",
                schema: "cleaners_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    type = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content = table.Column<string>(type: "jsonb", nullable: false),
                    processed_attempts = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_cleaners_next_run",
                schema: "cleaners_module",
                table: "cleaners",
                column: "next_run");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cleaners",
                schema: "cleaners_module");

            migrationBuilder.DropTable(
                name: "outbox",
                schema: "cleaners_module");
        }
    }
}
