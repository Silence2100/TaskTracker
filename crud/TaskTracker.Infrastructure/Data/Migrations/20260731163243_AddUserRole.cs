using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "password",
                table: "users",
                newName: "password_hash");

            migrationBuilder.RenameIndex(
                name: "IX_users_login",
                table: "users",
                newName: "ux_users_login");

            migrationBuilder.RenameIndex(
                name: "IX_users_email",
                table: "users",
                newName: "ux_users_email");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "password_hash",
                table: "users",
                newName: "password");

            migrationBuilder.RenameIndex(
                name: "ux_users_login",
                table: "users",
                newName: "IX_users_login");

            migrationBuilder.RenameIndex(
                name: "ux_users_email",
                table: "users",
                newName: "IX_users_email");
        }
    }
}
