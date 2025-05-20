using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProcessStep_BpmnProcess_BpmnProcessId",
                table: "ProcessStep");

            migrationBuilder.DropIndex(
                name: "IX_ProcessStep_BpmnProcessId",
                table: "ProcessStep");

            migrationBuilder.RenameColumn(
                name: "ResponsibleUserId",
                table: "ProcessStep",
                newName: "NextStepId");

            migrationBuilder.AddColumn<string>(
                name: "LastStepId",
                table: "ProcessStep",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BpmnProcessId = table.Column<int>(type: "int", nullable: false),
                    ProcessStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponsibleUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_ProcessStep_ProcessStepId",
                        column: x => x.ProcessStepId,
                        principalTable: "ProcessStep",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_ResponsibleUserId",
                        column: x => x.ResponsibleUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_bpmnProcessInstances_BpmnProcessId",
                        column: x => x.BpmnProcessId,
                        principalTable: "bpmnProcessInstances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BpmnProcessId",
                table: "Tasks",
                column: "BpmnProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProcessStepId",
                table: "Tasks",
                column: "ProcessStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ResponsibleUserId",
                table: "Tasks",
                column: "ResponsibleUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropColumn(
                name: "LastStepId",
                table: "ProcessStep");

            migrationBuilder.RenameColumn(
                name: "NextStepId",
                table: "ProcessStep",
                newName: "ResponsibleUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessStep_BpmnProcessId",
                table: "ProcessStep",
                column: "BpmnProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProcessStep_BpmnProcess_BpmnProcessId",
                table: "ProcessStep",
                column: "BpmnProcessId",
                principalTable: "BpmnProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
