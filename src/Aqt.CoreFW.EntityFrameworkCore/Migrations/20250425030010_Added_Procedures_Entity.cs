using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Added_Procedures_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppProcedures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    LastSyncedDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
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
                    table.PrimaryKey("PK_AppProcedures", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedures_Code",
                table: "AppProcedures",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedures_Name",
                table: "AppProcedures",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedures_Status",
                table: "AppProcedures",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedures_Status_Order_Name",
                table: "AppProcedures",
                columns: new[] { "Status", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedures_SyncRecordCode",
                table: "AppProcedures",
                column: "SyncRecordCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppProcedures");
        }
    }
}
