using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Added_Province_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppProvinces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    CountryId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    LastSyncedTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    SyncId = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    SyncCode = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
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
                    table.PrimaryKey("PK_AppProvinces", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppProvinces_AppCountries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "AppCountries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppProvinces_Code",
                table: "AppProvinces",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProvinces_CountryId",
                table: "AppProvinces",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProvinces_CountryId_Name",
                table: "AppProvinces",
                columns: new[] { "CountryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProvinces_CountryId_Order_Name",
                table: "AppProvinces",
                columns: new[] { "CountryId", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppProvinces_Status",
                table: "AppProvinces",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppProvinces");
        }
    }
}
