using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class migrationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubProcesses_BaselineHistories_BaselineHistoryId",
                table: "SubProcesses");

            migrationBuilder.AddForeignKey(
                name: "FK_SubProcesses_BaselineHistories_BaselineHistoryId",
                table: "SubProcesses",
                column: "BaselineHistoryId",
                principalTable: "BaselineHistories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubProcesses_BaselineHistories_BaselineHistoryId",
                table: "SubProcesses");

            migrationBuilder.AddForeignKey(
                name: "FK_SubProcesses_BaselineHistories_BaselineHistoryId",
                table: "SubProcesses",
                column: "BaselineHistoryId",
                principalTable: "BaselineHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
