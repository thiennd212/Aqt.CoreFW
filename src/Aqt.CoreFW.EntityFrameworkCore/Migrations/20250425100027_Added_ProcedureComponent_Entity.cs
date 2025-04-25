using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Added_ProcedureComponent_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppProcedureComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    FormDefinition = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    TempPath = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ExtraProperties = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProcedureComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppProcedureComponentLinks",
                columns: table => new
                {
                    ProcedureId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProcedureComponentId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProcedureComponentLinks", x => new { x.ProcedureId, x.ProcedureComponentId });
                    table.ForeignKey(
                        name: "FK_AppProcedureComponentLinks_AppProcedureComponents_ProcedureComponentId",
                        column: x => x.ProcedureComponentId,
                        principalTable: "AppProcedureComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponentLinks_ProcedureComponentId",
                table: "AppProcedureComponentLinks",
                column: "ProcedureComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponentLinks_ProcedureId",
                table: "AppProcedureComponentLinks",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Code",
                table: "AppProcedureComponents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Name",
                table: "AppProcedureComponents",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Status",
                table: "AppProcedureComponents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Status_Type_Order_Name",
                table: "AppProcedureComponents",
                columns: new[] { "Status", "Type", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Type",
                table: "AppProcedureComponents",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppProcedureComponentLinks");

            migrationBuilder.DropTable(
                name: "AppProcedureComponents");
        }
    }
}
