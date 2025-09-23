using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedById : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bpmnProcessInstances_Users_CreatedById",
                table: "bpmnProcessInstances");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "bpmnProcessInstances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_bpmnProcessInstances_Users_CreatedById",
                table: "bpmnProcessInstances",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bpmnProcessInstances_Users_CreatedById",
                table: "bpmnProcessInstances");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedById",
                table: "bpmnProcessInstances",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_bpmnProcessInstances_Users_CreatedById",
                table: "bpmnProcessInstances",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
