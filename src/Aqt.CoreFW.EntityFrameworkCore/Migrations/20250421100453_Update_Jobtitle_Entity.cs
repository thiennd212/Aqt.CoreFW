using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Update_Jobtitle_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncDate",
                table: "AppJobTitles",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncRecordCode",
                table: "AppJobTitles",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SyncRecordId",
                table: "AppJobTitles",
                type: "RAW(16)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSyncDate",
                table: "AppJobTitles");

            migrationBuilder.DropColumn(
                name: "SyncRecordCode",
                table: "AppJobTitles");

            migrationBuilder.DropColumn(
                name: "SyncRecordId",
                table: "AppJobTitles");
        }
    }
}
