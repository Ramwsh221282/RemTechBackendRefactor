using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Adapter.Storage.Migrations
{
    /// <inheritdoc />
    public partial class Identity_User_Tickets_Added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_roles_users_user_id",
                schema: "users_module",
                table: "user_roles");

            migrationBuilder.CreateTable(
                name: "tickets",
                schema: "users_module",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expired = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tickets", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_tickets",
                        column: x => x.user_id,
                        principalSchema: "users_module",
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_user_id",
                schema: "users_module",
                table: "tickets",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_users_user_roles",
                schema: "users_module",
                table: "user_roles",
                column: "user_id",
                principalSchema: "users_module",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_users_user_roles",
                schema: "users_module",
                table: "user_roles");

            migrationBuilder.DropTable(
                name: "tickets",
                schema: "users_module");

            migrationBuilder.AddForeignKey(
                name: "FK_user_roles_users_user_id",
                schema: "users_module",
                table: "user_roles",
                column: "user_id",
                principalSchema: "users_module",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
