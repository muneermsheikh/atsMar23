using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class EmploymentChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompanyName",
                table: "Employments",
                newName: "AgentName");

            migrationBuilder.AddColumn<bool>(
                name: "Approved",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ApprovedByEmpId",
                table: "Employments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "Employments",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Approved",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "ApprovedByEmpId",
                table: "Employments");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "Employments");

            migrationBuilder.RenameColumn(
                name: "AgentName",
                table: "Employments",
                newName: "CompanyName");
        }
    }
}
