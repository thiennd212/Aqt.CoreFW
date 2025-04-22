using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Added_File_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TenantId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    ParentId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    FileContainerName = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    FileName = table.Column<string>(type: "NVARCHAR2(255)", maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(type: "NVARCHAR2(128)", maxLength: 128, nullable: true),
                    FileType = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    SubFilesQuantity = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    HasSubdirectories = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    ByteSize = table.Column<long>(type: "NUMBER(19)", nullable: false),
                    Hash = table.Column<string>(type: "NVARCHAR2(32)", maxLength: 32, nullable: true),
                    BlobName = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    Flag = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    SoftDeletionToken = table.Column<string>(type: "NVARCHAR2(450)", nullable: true, defaultValue: ""),
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
                    table.PrimaryKey("PK_AppFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppFileUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    TenantId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    UserName = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR2(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "NVARCHAR2(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR2(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "BOOLEAN", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "NVARCHAR2(2000)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "NVARCHAR2(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFileUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFiles_BlobName",
                table: "AppFiles",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_AppFiles_FileName_ParentId_OwnerUserId_FileContainerName_TenantId_SoftDeletionToken",
                table: "AppFiles",
                columns: new[] { "FileName", "ParentId", "OwnerUserId", "FileContainerName", "TenantId", "SoftDeletionToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppFiles_Hash",
                table: "AppFiles",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_AppFiles_ParentId",
                table: "AppFiles",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppFiles_ParentId_OwnerUserId_FileContainerName_FileName",
                table: "AppFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName", "FileName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFiles");

            migrationBuilder.DropTable(
                name: "AppFileUsers");
        }
    }
}
