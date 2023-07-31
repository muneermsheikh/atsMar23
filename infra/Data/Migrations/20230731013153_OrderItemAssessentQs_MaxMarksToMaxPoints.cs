using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class OrderItemAssessentQsMaxMarksToMaxPoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxMarks",
                table: "OrderItemAssessmentQs",
                newName: "MaxPoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaxPoints",
                table: "OrderItemAssessmentQs",
                newName: "MaxMarks");
        }
    }
}
