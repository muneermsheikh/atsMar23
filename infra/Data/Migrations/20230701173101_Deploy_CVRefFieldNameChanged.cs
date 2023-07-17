using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class DeployCVRefFieldNameChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys");

            migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId1",
                table: "Deploys");

            migrationBuilder.DropIndex(
                name: "IX_Deploys_CVRefId1",
                table: "Deploys");

            migrationBuilder.DropColumn(
                name: "CVRefId1",
                table: "Deploys");

            migrationBuilder.AlterColumn<int>(
                name: "CVRefId",
                table: "Deploys",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "DeployCVRefId",
                table: "Deploys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Deploys_DeployCVRefId",
                table: "Deploys",
                column: "DeployCVRefId");

            migrationBuilder.AddForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys",
                column: "CVRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Deploys_CVRefs_DeployCVRefId",
                table: "Deploys",
                column: "DeployCVRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys");

            migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_DeployCVRefId",
                table: "Deploys");

            migrationBuilder.DropIndex(
                name: "IX_Deploys_DeployCVRefId",
                table: "Deploys");

            migrationBuilder.DropColumn(
                name: "DeployCVRefId",
                table: "Deploys");

            migrationBuilder.AlterColumn<int>(
                name: "CVRefId",
                table: "Deploys",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CVRefId1",
                table: "Deploys",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Deploys_CVRefId1",
                table: "Deploys",
                column: "CVRefId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys",
                column: "CVRefId",
                principalTable: "CVRefs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId1",
                table: "Deploys",
                column: "CVRefId1",
                principalTable: "CVRefs",
                principalColumn: "Id");
        }
    }
}
