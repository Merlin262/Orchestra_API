using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByForeignKeyToBpmnProcessBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BpmnProcess",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "BpmnProcess",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BpmnProcess_CreatedByUserId",
                table: "BpmnProcess",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BpmnProcess_Users_CreatedByUserId",
                table: "BpmnProcess",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BpmnProcess_Users_CreatedByUserId",
                table: "BpmnProcess");

            migrationBuilder.DropIndex(
                name: "IX_BpmnProcess_CreatedByUserId",
                table: "BpmnProcess");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BpmnProcess");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "BpmnProcess");
        }
    }
}
