using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Update_OrganizatioUnit_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "AbpOrganizationUnits",
                type: "NVARCHAR2(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastSyncedTime",
                table: "AbpOrganizationUnits",
                type: "TIMESTAMP(7)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ManualCode",
                table: "AbpOrganizationUnits",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "AbpOrganizationUnits",
                type: "NUMBER(10)",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "Status",
                table: "AbpOrganizationUnits",
                type: "NUMBER(3)",
                nullable: false,
                defaultValue: (byte)1);

            migrationBuilder.AddColumn<string>(
                name: "SyncRecordCode",
                table: "AbpOrganizationUnits",
                type: "NVARCHAR2(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SyncRecordId",
                table: "AbpOrganizationUnits",
                type: "NVARCHAR2(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "LastSyncedTime",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "ManualCode",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "SyncRecordCode",
                table: "AbpOrganizationUnits");

            migrationBuilder.DropColumn(
                name: "SyncRecordId",
                table: "AbpOrganizationUnits");
        }
    }
}
