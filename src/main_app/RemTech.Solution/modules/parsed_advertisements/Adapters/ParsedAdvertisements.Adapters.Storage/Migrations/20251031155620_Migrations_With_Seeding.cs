using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParsedAdvertisements.Adapters.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Migrations_With_Seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "kind",
                schema: "parsed_advertisements_module",
                table: "regions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kind",
                schema: "parsed_advertisements_module",
                table: "regions");
        }
    }
}
