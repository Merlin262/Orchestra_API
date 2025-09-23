using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Orchestra.Migrations
{
    /// <inheritdoc />
    public partial class InsertBpmnType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BpmnItem",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 0, "Unknown" },
                    { 1, "Start Event" },
                    { 2, "End Event" },
                    { 3, "User Task" },
                    { 4, "Service Task" },
                    { 5, "Script Task" },
                    { 6, "Manual Task" },
                    { 7, "Exclusive Gateway" },
                    { 8, "Parallel Gateway" },
                    { 9, "Inclusive Gateway" },
                    { 10, "Intermediate Throw Event" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int id = 0; id <= 10; id++)
            {
                migrationBuilder.DeleteData(
                    table: "BpmnItem",
                    keyColumn: "Id",
                    keyValue: id);
            }
        }
    }
}
