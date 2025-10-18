using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class addHistoryBaselineId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BaselineHistoryId",
                table: "SubProcesses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubProcesses_BaselineHistoryId",
                table: "SubProcesses",
                column: "BaselineHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubProcesses_BaselineHistories_BaselineHistoryId",
                table: "SubProcesses",
                column: "BaselineHistoryId",
                principalTable: "BaselineHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction); // ADICIONE ESTA LINHA
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubProcesses_BaselineHistories_BaselineHistoryId",
                table: "SubProcesses");

            migrationBuilder.DropIndex(
                name: "IX_SubProcesses_BaselineHistoryId",
                table: "SubProcesses");

            migrationBuilder.DropColumn(
                name: "BaselineHistoryId",
                table: "SubProcesses");
        }
    }
}
