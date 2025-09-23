using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class AddCreatedByToInstance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "bpmnProcessInstances",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_bpmnProcessInstances_CreatedById",
                table: "bpmnProcessInstances",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_bpmnProcessInstances_Users_CreatedById",
                table: "bpmnProcessInstances",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_bpmnProcessInstances_Users_CreatedById",
                table: "bpmnProcessInstances");

            migrationBuilder.DropIndex(
                name: "IX_bpmnProcessInstances_CreatedById",
                table: "bpmnProcessInstances");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "bpmnProcessInstances");
        }
    }
}
