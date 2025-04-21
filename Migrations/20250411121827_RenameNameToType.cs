using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyPhongNet.Migrations
{
    /// <inheritdoc />
    public partial class RenameNameToType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Devices",
                newName: "Type");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "UsageTime",
                table: "UsageRecords",
                type: "time",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PricePerHour",
                table: "Devices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsageTime",
                table: "UsageRecords");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Devices",
                newName: "Name");

            migrationBuilder.AlterColumn<double>(
                name: "PricePerHour",
                table: "Devices",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
