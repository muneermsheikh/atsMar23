using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class VoucherEntryApprovals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "FinanceVouchers");

            migrationBuilder.DropColumn(
                name: "ReviewedById",
                table: "FinanceVouchers");

            migrationBuilder.DropColumn(
                name: "ReviewedByName",
                table: "FinanceVouchers");

            migrationBuilder.DropColumn(
                name: "ReviewedOn",
                table: "FinanceVouchers");

            migrationBuilder.AddColumn<bool>(
                name: "DrEntryApproved",
                table: "VoucherEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DrEntryApprovedByEmployeeById",
                table: "VoucherEntries",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DrEntryApprovedOn",
                table: "VoucherEntries",
                type: "TEXT",
                maxLength: 10,
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DrEntryApproved",
                table: "VoucherEntries");

            migrationBuilder.DropColumn(
                name: "DrEntryApprovedByEmployeeById",
                table: "VoucherEntries");

            migrationBuilder.DropColumn(
                name: "DrEntryApprovedOn",
                table: "VoucherEntries");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "FinanceVouchers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ReviewedById",
                table: "FinanceVouchers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReviewedByName",
                table: "FinanceVouchers",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedOn",
                table: "FinanceVouchers",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
