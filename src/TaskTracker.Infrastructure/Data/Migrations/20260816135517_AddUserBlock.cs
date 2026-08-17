using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserBlock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "blocked",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "blocked",
                table: "users");
        }
    }
}
