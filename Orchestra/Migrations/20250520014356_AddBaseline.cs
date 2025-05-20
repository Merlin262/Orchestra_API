using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStep_BpmnItem_BpmnItemId",
                table: "ProcessStep");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStep_BpmnItemId",
                table: "ProcessStep");

            migrationBuilder.DropColumn(
                name: "BpmnItemId",
                table: "ProcessStep");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ProcessStep",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "ProcessStep");

            migrationBuilder.AddColumn<int>(
                name: "BpmnItemId",
                table: "ProcessStep",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStep_BpmnItemId",
                table: "ProcessStep",
                column: "BpmnItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStep_BpmnItem_BpmnItemId",
                table: "ProcessStep",
                column: "BpmnItemId",
                principalTable: "BpmnItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
