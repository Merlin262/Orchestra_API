using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddSubProcessTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SubProcessId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubProcesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ProcessBaselineId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProcesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubProcesses_BpmnProcess_ProcessBaselineId",
                        column: x => x.ProcessBaselineId,
                        principalTable: "BpmnProcess",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubProcesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_SubProcessId",
                table: "Tasks",
                column: "SubProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProcesses_ProcessBaselineId",
                table: "SubProcesses",
                column: "ProcessBaselineId");

            migrationBuilder.CreateIndex(
                name: "IX_SubProcesses_UserId",
                table: "SubProcesses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_SubProcesses_SubProcessId",
                table: "Tasks",
                column: "SubProcessId",
                principalTable: "SubProcesses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_SubProcesses_SubProcessId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "SubProcesses");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_SubProcessId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "SubProcessId",
                table: "Tasks");
        }
    }
}
