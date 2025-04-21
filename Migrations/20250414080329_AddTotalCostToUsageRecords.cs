using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongNet.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalCostToUsageRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "UsageRecords",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "UsageRecords");
        }
    }
}
