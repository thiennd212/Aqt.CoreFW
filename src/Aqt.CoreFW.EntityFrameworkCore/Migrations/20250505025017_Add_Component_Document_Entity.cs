using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aqt.CoreFW.Migrations
{
    /// <inheritdoc />
    public partial class Add_Component_Document_Entity : Migration
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
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    ApplicantName = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    ApplicantIdentityNumber = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: true),
                    ApplicantAddress = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
                    ApplicantEmail = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: true),
                    ApplicantPhoneNumber = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: true),
                    ScopeOfActivity = table.Column<string>(type: "NVARCHAR2(2000)", nullable: true),
                    ReceiveByPost = table.Column<bool>(type: "BOOLEAN", nullable: false),
                    WorkflowStatusId = table.Column<Guid>(type: "RAW(16)", nullable: true),
                    SubmissionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ReceptionDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    ResultDate = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: true),
                    RejectionOrAdditionReason = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
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
                        name: "FK_AppBDocuments_AppWorkflowStatuses_WorkflowStatusId",
                        column: x => x.WorkflowStatusId,
                        principalTable: "AppWorkflowStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppProcedureComponents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    Code = table.Column<string>(type: "NVARCHAR2(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR2(250)", maxLength: 250, nullable: false),
                    Status = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    Order = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    Description = table.Column<string>(type: "NVARCHAR2(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<byte>(type: "NUMBER(3)", nullable: false),
                    FormDefinition = table.Column<string>(type: "NCLOB", nullable: true),
                    TempPath = table.Column<string>(type: "NVARCHAR2(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_AppProcedureComponents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppBDocumentData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    BDocumentId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProcedureComponentId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    InputData = table.Column<string>(type: "NCLOB", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "AppProcedureComponentLinks",
                columns: table => new
                {
                    ProcedureId = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ProcedureComponentId = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppProcedureComponentLinks", x => new { x.ProcedureId, x.ProcedureComponentId });
                    table.ForeignKey(
                        name: "FK_AppProcedureComponentLinks_AppProcedureComponents_ProcedureComponentId",
                        column: x => x.ProcedureComponentId,
                        principalTable: "AppProcedureComponents",
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
                name: "IX_AppBDocuments_ApplicantName",
                table: "AppBDocuments",
                column: "ApplicantName");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_Code",
                table: "AppBDocuments",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_CreationTime",
                table: "AppBDocuments",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_Proc_Status_Created",
                table: "AppBDocuments",
                columns: new[] { "ProcedureId", "WorkflowStatusId", "CreationTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_ProcedureId",
                table: "AppBDocuments",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_ReceiveByPost",
                table: "AppBDocuments",
                column: "ReceiveByPost");

            migrationBuilder.CreateIndex(
                name: "IX_AppBDocuments_WorkflowStatusId",
                table: "AppBDocuments",
                column: "WorkflowStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponentLinks_ProcedureComponentId",
                table: "AppProcedureComponentLinks",
                column: "ProcedureComponentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponentLinks_ProcedureId",
                table: "AppProcedureComponentLinks",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Code",
                table: "AppProcedureComponents",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Name",
                table: "AppProcedureComponents",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Status",
                table: "AppProcedureComponents",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Status_Type_Order_Name",
                table: "AppProcedureComponents",
                columns: new[] { "Status", "Type", "Order", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_AppProcedureComponents_Type",
                table: "AppProcedureComponents",
                column: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppBDocumentData");

            migrationBuilder.DropTable(
                name: "AppProcedureComponentLinks");

            migrationBuilder.DropTable(
                name: "AppBDocuments");

            migrationBuilder.DropTable(
                name: "AppProcedureComponents");
        }
    }
}
