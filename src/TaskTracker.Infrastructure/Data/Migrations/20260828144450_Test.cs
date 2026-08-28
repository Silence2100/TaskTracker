using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskTracker.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_projects_project_id",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_project_members",
                table: "project_members");

            migrationBuilder.DropIndex(
                name: "IX_project_members_project_id",
                table: "project_members");

            migrationBuilder.RenameColumn(
                name: "project_id",
                table: "tasks",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_project_id",
                table: "tasks",
                newName: "IX_tasks_ProjectId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ProjectId",
                table: "tasks",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_project_members",
                table: "project_members",
                columns: new[] { "project_id", "user_id" });

            migrationBuilder.CreateIndex(
                name: "IX_project_members_user_id_project_id",
                table: "project_members",
                columns: new[] { "user_id", "project_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_projects_ProjectId",
                table: "tasks",
                column: "ProjectId",
                principalTable: "projects",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_projects_ProjectId",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_project_members",
                table: "project_members");

            migrationBuilder.DropIndex(
                name: "IX_project_members_user_id_project_id",
                table: "project_members");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "tasks",
                newName: "project_id");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_ProjectId",
                table: "tasks",
                newName: "IX_tasks_project_id");

            migrationBuilder.AlterColumn<Guid>(
                name: "project_id",
                table: "tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_project_members",
                table: "project_members",
                columns: new[] { "user_id", "project_id" });

            migrationBuilder.CreateIndex(
                name: "IX_project_members_project_id",
                table: "project_members",
                column: "project_id");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_projects_project_id",
                table: "tasks",
                column: "project_id",
                principalTable: "projects",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}