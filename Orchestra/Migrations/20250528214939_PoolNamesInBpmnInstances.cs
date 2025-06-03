using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class PoolNamesInBpmnInstances : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PoolNames",
                table: "bpmnProcessInstances",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PoolNames",
                table: "bpmnProcessInstances");
        }
    }
}
