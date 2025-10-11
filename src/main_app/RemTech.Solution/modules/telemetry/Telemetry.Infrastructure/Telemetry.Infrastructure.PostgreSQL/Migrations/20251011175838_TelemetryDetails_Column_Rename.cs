using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Telemetry.Infrastructure.PostgreSQL.Migrations
{
    /// <inheritdoc />
    public partial class TelemetryDetails_Column_Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                schema: "telemetry_module",
                table: "records",
                newName: "details");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "details",
                schema: "telemetry_module",
                table: "records",
                newName: "Details");
        }
    }
}
