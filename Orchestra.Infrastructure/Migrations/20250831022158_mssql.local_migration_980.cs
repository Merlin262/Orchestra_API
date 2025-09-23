using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class mssqllocal_migration_980 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedByUserId",
                table: "TaskFiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "TaskFiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskFiles_UserId",
                table: "TaskFiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFiles_Users_UserId",
                table: "TaskFiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFiles_Users_UserId",
                table: "TaskFiles");

            migrationBuilder.DropIndex(
                name: "IX_TaskFiles_UserId",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskFiles");
        }
    }
}
