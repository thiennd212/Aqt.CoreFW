using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Added_DataCore_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDataCores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    DataGroupId = table.Column<Guid>(type: "RAW(16)", nullable: false),
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
                    table.PrimaryKey("PK_AppDataCores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDataCores_AppDataGroups_DataGroupId",
                        column: x => x.DataGroupId,
                        principalTable: "AppDataGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDataCores_Code",
                table: "AppDataCores",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDataCores_DataGroupId",
                table: "AppDataCores",
                column: "DataGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDataCores_DataGroupId_Status_Order_Name",
                table: "AppDataCores",
                columns: new[] { "DataGroupId", "Status", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppDataCores_Name",
                table: "AppDataCores",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppDataCores_Status",
                table: "AppDataCores",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDataCores");
        }
    }
}
