using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Added_Commune_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppCommunes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false, defaultValue: 0),
                    Description = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ProvinceId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DistrictId = table.Column<Guid>(type: "RAW(16)", nullable: true),
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
                    table.PrimaryKey("PK_AppCommunes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppCommunes_AppDistricts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "AppDistricts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppCommunes_AppProvinces_ProvinceId",
                        column: x => x.ProvinceId,
                        principalTable: "AppProvinces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppCommunes_Code",
                table: "AppCommunes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppCommunes_DistrictId",
                table: "AppCommunes",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCommunes_Name",
                table: "AppCommunes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppCommunes_ProvinceId",
                table: "AppCommunes",
                column: "ProvinceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppCommunes_ProvinceId_DistrictId_Order_Name",
                table: "AppCommunes",
                columns: new[] { "ProvinceId", "DistrictId", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppCommunes_Status",
                table: "AppCommunes",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppCommunes");
        }
    }
}
