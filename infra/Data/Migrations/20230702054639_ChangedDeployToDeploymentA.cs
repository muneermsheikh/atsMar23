using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedDeployToDeploymentA : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys");

            migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_DeployCVRefId",
                table: "Deploys");

            migrationBuilder.DropIndex(
                name: "IX_Deploys_DeployCVRefId",
                table: "Deploys");
            */

            migrationBuilder.CreateTable(
                name: "Deployments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeployCVRefId = table.Column<int>(type: "INTEGER", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextSequence = table.Column<int>(type: "INTEGER", nullable: false),
                    NextStageDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CVRefId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deployments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deployments_CVRefs_CVRefId",
                        column: x => x.CVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Deployments_CVRefs_DeployCVRefId",
                        column: x => x.DeployCVRefId,
                        principalTable: "CVRefs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_CVRefId",
                table: "Deployments",
                column: "CVRefId");

            migrationBuilder.CreateIndex(
                name: "IX_Deployments_DeployCVRefId",
                table: "Deployments",
                column: "DeployCVRefId");

            /*migrationBuilder.AddForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys",
                column: "CVRefId",
                principalTable: "CVRefs",
                principalColumn: "Id");
            */
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            /* migrationBuilder.DropForeignKey(
                name: "FK_Deploys_CVRefs_CVRefId",
                table: "Deploys");
            */
            migrationBuilder.DropTable(
                name: "Deployments");

            /* migrationBuilder.CreateIndex(
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
            */
        }
    }
}
