using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class FixStatusOneMoreTime1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "SubTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_StatusId",
                table: "SubTasks",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubTasks_Status_StatusId",
                table: "SubTasks",
                column: "StatusId",
                principalTable: "Status",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubTasks_Status_StatusId",
                table: "SubTasks");

            migrationBuilder.DropIndex(
                name: "IX_SubTasks_StatusId",
                table: "SubTasks");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "SubTasks");
        }
    }
}
