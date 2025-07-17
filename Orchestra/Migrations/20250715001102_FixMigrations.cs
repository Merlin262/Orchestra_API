using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class FixMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bpmnProcessInstances_BpmnProcess_BpmnProcessBaselineId",
                table: "bpmnProcessInstances");

            migrationBuilder.AddColumn<double>(
                name: "version",
                table: "bpmnProcessInstances",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AlterColumn<double>(
                name: "Version",
                table: "BpmnProcess",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bpmnProcessInstances_BpmnProcess_BpmnProcessBaselineId",
                table: "bpmnProcessInstances",
                column: "BpmnProcessBaselineId",
                principalTable: "BpmnProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bpmnProcessInstances_BpmnProcess_BpmnProcessBaselineId",
                table: "bpmnProcessInstances");

            migrationBuilder.DropColumn(
                name: "version",
                table: "bpmnProcessInstances");

            migrationBuilder.AlterColumn<double>(
                name: "Version",
                table: "BpmnProcess",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddForeignKey(
                name: "FK_bpmnProcessInstances_BpmnProcess_BpmnProcessBaselineId",
                table: "bpmnProcessInstances",
                column: "BpmnProcessBaselineId",
                principalTable: "BpmnProcess",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
