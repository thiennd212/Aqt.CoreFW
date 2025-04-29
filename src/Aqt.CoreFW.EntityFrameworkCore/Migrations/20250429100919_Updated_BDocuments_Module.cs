using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Updated_BDocuments_Module : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppBDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProcedureId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    MaHoSo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    TenChuHoSo = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    SoDinhDanhChuHoSo = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    DiaChiChuHoSo = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    EmailChuHoSo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    SoDienThoaiChuHoSo = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    PhamViHoatDong = table.Column<string>(type: "NCLOB", nullable: true),
                    DangKyNhanQuaBuuDien = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    TrangThaiHoSoId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    NgayNop = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NgayTiepNhan = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NgayHenTra = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    NgayTraKetQua = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    LyDoTuChoiHoacBoSung = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
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
                    table.PrimaryKey("PK_AppBDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBDocuments_AppProcedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "AppProcedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AppBDocuments_AppWorkflowStatuses_TrangThaiHoSoId",
                        column: x => x.TrangThaiHoSoId,
                        principalTable: "AppWorkflowStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppBDocumentData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BDocumentId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProcedureComponentId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    DuLieuNhap = table.Column<string>(type: "NCLOB", nullable: true),
                    FileId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "RAW(16)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppBDocumentData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppBDocumentData_AppBDocuments_BDocumentId",
                        column: x => x.BDocumentId,
                        principalTable: "AppBDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocumentData_BDocumentId",
                table: "AppBDocumentData",
                column: "BDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocumentData_Doc_Comp",
                table: "AppBDocumentData",
                columns: new[] { "BDocumentId", "ProcedureComponentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocumentData_FileId",
                table: "AppBDocumentData",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocumentData_ProcedureComponentId",
                table: "AppBDocumentData",
                column: "ProcedureComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_CreationTime",
                table: "AppBDocuments",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_DangKyBuuDien",
                table: "AppBDocuments",
                column: "DangKyNhanQuaBuuDien");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_MaHoSo",
                table: "AppBDocuments",
                column: "MaHoSo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_Proc_Status_Created",
                table: "AppBDocuments",
                columns: new[] { "ProcedureId", "TrangThaiHoSoId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_ProcedureId",
                table: "AppBDocuments",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_TenChuHoSo",
                table: "AppBDocuments",
                column: "TenChuHoSo");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_TrangThaiHoSoId",
                table: "AppBDocuments",
                column: "TrangThaiHoSoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBDocumentData");

            migrationBuilder.DropTable(
                name: "AppBDocuments");
        }
    }
}
