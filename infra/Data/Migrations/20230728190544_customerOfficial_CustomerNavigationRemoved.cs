using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class customerOfficialCustomerNavigationRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOfficials_Customers_CustomerId1",
                table: "CustomerOfficials");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOfficials_CustomerId1",
                table: "CustomerOfficials");

            migrationBuilder.DropColumn(
                name: "CustomerId1",
                table: "CustomerOfficials");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "Introduction",
                table: "Candidates");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CustomerId1",
                table: "CustomerOfficials",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Candidates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Introduction",
                table: "Candidates",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOfficials_CustomerId1",
                table: "CustomerOfficials",
                column: "CustomerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOfficials_Customers_CustomerId1",
                table: "CustomerOfficials",
                column: "CustomerId1",
                principalTable: "Customers",
                principalColumn: "Id");
        }
    }
}
