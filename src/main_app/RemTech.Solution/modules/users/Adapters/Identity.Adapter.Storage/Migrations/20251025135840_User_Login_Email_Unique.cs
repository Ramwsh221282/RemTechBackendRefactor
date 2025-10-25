using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Adapter.Storage.Migrations
{
    /// <inheritdoc />
    public partial class User_Login_Email_Unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_users_email_unique",
                schema: "users_module",
                table: "users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_users_login_unique",
                schema: "users_module",
                table: "users",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_email_unique",
                schema: "users_module",
                table: "users");

            migrationBuilder.DropIndex(
                name: "idx_users_login_unique",
                schema: "users_module",
                table: "users");
        }
    }
}
