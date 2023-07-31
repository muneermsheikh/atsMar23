using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class StddAssessmentQQNotoQuestionNo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QNo",
                table: "AssessmentStandardQs",
                newName: "QuestionNo");

            migrationBuilder.RenameIndex(
                name: "IX_AssessmentStandardQs_QNo",
                table: "AssessmentStandardQs",
                newName: "IX_AssessmentStandardQs_QuestionNo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuestionNo",
                table: "AssessmentStandardQs",
                newName: "QNo");

            migrationBuilder.RenameIndex(
                name: "IX_AssessmentStandardQs_QuestionNo",
                table: "AssessmentStandardQs",
                newName: "IX_AssessmentStandardQs_QNo");
        }
    }
}
