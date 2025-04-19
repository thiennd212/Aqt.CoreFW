using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Add_WorkflowStatus_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppWorkflowStatuses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ColorCode = table.Column<string>(type: "NVARCHAR2(7)", maxLength: 7, nullable: true),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
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
                    table.PrimaryKey("PK_AppWorkflowStatuses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStatuses_Code",
                table: "AppWorkflowStatuses",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStatuses_IsActive",
                table: "AppWorkflowStatuses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStatuses_Name",
                table: "AppWorkflowStatuses",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppWorkflowStatuses_Order_Name",
                table: "AppWorkflowStatuses",
                columns: new[] { "Order", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppWorkflowStatuses");
        }
    }
}
