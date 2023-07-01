using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class CVRefSequences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextStageId",
                table: "CVRefs");

            migrationBuilder.AddColumn<int>(
                name: "NextSequence",
                table: "CVRefs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "CVRefs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NextSequence",
                table: "CVRefs");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "CVRefs");

            migrationBuilder.AddColumn<int>(
                name: "NextStageId",
                table: "CVRefs",
                type: "INTEGER",
                nullable: true);
        }
    }
}
