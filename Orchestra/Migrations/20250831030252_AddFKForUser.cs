using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddFKForUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedByUserId",
                table: "TaskFiles",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TaskFiles_UploadedByUserId",
                table: "TaskFiles",
                column: "UploadedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskFiles_Users_UploadedByUserId",
                table: "TaskFiles",
                column: "UploadedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskFiles_Users_UploadedByUserId",
                table: "TaskFiles");

            migrationBuilder.DropIndex(
                name: "IX_TaskFiles_UploadedByUserId",
                table: "TaskFiles");

            migrationBuilder.DropColumn(
                name: "UploadedByUserId",
                table: "TaskFiles");
        }
    }
}
