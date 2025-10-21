using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Adapter.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Identity_Unique_Role_Name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_roles_name",
                schema: "users_module",
                table: "roles",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_roles_name",
                schema: "users_module",
                table: "roles");
        }
    }
}
