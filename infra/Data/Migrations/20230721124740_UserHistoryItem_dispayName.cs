using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserHistoryItemdispayName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                table: "UserHistoryItems");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "UserHistories");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                table: "UserHistoryItems",
                column: "UserHistoryId",
                principalTable: "UserHistories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                table: "UserHistoryItems");

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "UserHistories",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserHistoryItems_UserHistories_UserHistoryId",
                table: "UserHistoryItems",
                column: "UserHistoryId",
                principalTable: "UserHistories",
                principalColumn: "Id");
        }
    }
}
