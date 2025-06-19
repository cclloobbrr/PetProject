using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Test_task.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SMP",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SMP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supervisory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Checks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateStart = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    DateFinish = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "GETDATE()"),
                    PlannedDuration = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SMPId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SupervisoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checks_SMP_SMPId",
                        column: x => x.SMPId,
                        principalTable: "SMP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checks_Supervisory_SupervisoryId",
                        column: x => x.SupervisoryId,
                        principalTable: "Supervisory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Checks_SMPId",
                table: "Checks",
                column: "SMPId");

            migrationBuilder.CreateIndex(
                name: "IX_Checks_SupervisoryId",
                table: "Checks",
                column: "SupervisoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Checks");

            migrationBuilder.DropTable(
                name: "SMP");

            migrationBuilder.DropTable(
                name: "Supervisory");
        }
    }
}
