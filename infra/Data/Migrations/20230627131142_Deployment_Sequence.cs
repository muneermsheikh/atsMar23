using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeploymentSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StageId",
                table: "Deploys",
                newName: "Sequence");

            migrationBuilder.RenameColumn(
                name: "NextStageId",
                table: "Deploys",
                newName: "NextSequence");

            migrationBuilder.RenameColumn(
                name: "NextEstimatedStageDate",
                table: "Deploys",
                newName: "NextStageDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sequence",
                table: "Deploys",
                newName: "StageId");

            migrationBuilder.RenameColumn(
                name: "NextStageDate",
                table: "Deploys",
                newName: "NextEstimatedStageDate");

            migrationBuilder.RenameColumn(
                name: "NextSequence",
                table: "Deploys",
                newName: "NextStageId");
        }
    }
}
