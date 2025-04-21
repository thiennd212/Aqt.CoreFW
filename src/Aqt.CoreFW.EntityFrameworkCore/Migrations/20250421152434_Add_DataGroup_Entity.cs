using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Add_DataGroup_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppDataGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ParentId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    LastSyncDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    SyncRecordId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    SyncRecordCode = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
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
                    table.PrimaryKey("PK_AppDataGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppDataGroups_AppDataGroups_ParentId",
                        column: x => x.ParentId,
                        principalTable: "AppDataGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppDataGroups_Code",
                table: "AppDataGroups",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppDataGroups_Name",
                table: "AppDataGroups",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppDataGroups_ParentId",
                table: "AppDataGroups",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppDataGroups_ParentId_Status_Order_Name",
                table: "AppDataGroups",
                columns: new[] { "ParentId", "Status", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppDataGroups_Status",
                table: "AppDataGroups",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppDataGroups");
        }
    }
}
