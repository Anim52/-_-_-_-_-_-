using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaxiMed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "ClinicBranch",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicBranch", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diagnosis",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnosis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Patient",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LastName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PriceList",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    BasePrice = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Specialty",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specialty", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InsurancePolicy",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    PolicyNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsurancePolicy", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InsurancePolicy_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "dbo",
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatientDocument",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Series = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Number = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IssuedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientDocument_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "dbo",
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PriceListItem",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PriceListId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceListItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriceListItem_PriceList_PriceListId",
                        column: x => x.PriceListId,
                        principalSchema: "dbo",
                        principalTable: "PriceList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriceListItem_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Doctor",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SpecialtyId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    Room = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Doctor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Doctor_ClinicBranch_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "ClinicBranch",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Doctor_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalSchema: "dbo",
                        principalTable: "Specialty",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLog",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    At = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Entity = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    DetailsJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLog_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "dbo",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "dbo",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    StartAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointment_ClinicBranch_BranchId",
                        column: x => x.BranchId,
                        principalSchema: "dbo",
                        principalTable: "ClinicBranch",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointment_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "dbo",
                        principalTable: "Doctor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "dbo",
                        principalTable: "Patient",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointment_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalSchema: "dbo",
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DoctorSchedule",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SlotMinutes = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DoctorSchedule_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "dbo",
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentService",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppointmentService_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "dbo",
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentService_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoice_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "dbo",
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invoice_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "dbo",
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Visit",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AppointmentId = table.Column<long>(type: "bigint", nullable: false),
                    DoctorId = table.Column<int>(type: "int", nullable: false),
                    OpenedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Complaints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Anamnesis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Examination = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiagnosisText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Recommendations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Visit_Appointment_AppointmentId",
                        column: x => x.AppointmentId,
                        principalSchema: "dbo",
                        principalTable: "Appointment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Visit_Doctor_DoctorId",
                        column: x => x.DoctorId,
                        principalSchema: "dbo",
                        principalTable: "Doctor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceItem",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(12,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "dbo",
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvoiceItem_Service_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "dbo",
                        principalTable: "Service",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<long>(type: "bigint", nullable: false),
                    Method = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payment_Invoice_InvoiceId",
                        column: x => x.InvoiceId,
                        principalSchema: "dbo",
                        principalTable: "Invoice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attachment",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    VisitId = table.Column<long>(type: "bigint", nullable: true),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    StoragePath = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    FileBlob = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attachment_Patient_PatientId",
                        column: x => x.PatientId,
                        principalSchema: "dbo",
                        principalTable: "Patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attachment_Visit_VisitId",
                        column: x => x.VisitId,
                        principalSchema: "dbo",
                        principalTable: "Visit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prescription", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prescription_Visit_VisitId",
                        column: x => x.VisitId,
                        principalSchema: "dbo",
                        principalTable: "Visit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisitDiagnosis",
                schema: "dbo",
                columns: table => new
                {
                    VisitId = table.Column<long>(type: "bigint", nullable: false),
                    DiagnosisId = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitDiagnosis", x => new { x.VisitId, x.DiagnosisId });
                    table.ForeignKey(
                        name: "FK_VisitDiagnosis_Diagnosis_DiagnosisId",
                        column: x => x.DiagnosisId,
                        principalSchema: "dbo",
                        principalTable: "Diagnosis",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisitDiagnosis_Visit_VisitId",
                        column: x => x.VisitId,
                        principalSchema: "dbo",
                        principalTable: "Visit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_BranchId_StartAt",
                schema: "dbo",
                table: "Appointment",
                columns: new[] { "BranchId", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_CreatedByUserId",
                schema: "dbo",
                table: "Appointment",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_DoctorId_StartAt",
                schema: "dbo",
                table: "Appointment",
                columns: new[] { "DoctorId", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_PatientId_StartAt",
                schema: "dbo",
                table: "Appointment",
                columns: new[] { "PatientId", "StartAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_AppointmentId",
                schema: "dbo",
                table: "AppointmentService",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentService_ServiceId",
                schema: "dbo",
                table: "AppointmentService",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_PatientId",
                schema: "dbo",
                table: "Attachment",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_VisitId",
                schema: "dbo",
                table: "Attachment",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_At",
                schema: "dbo",
                table: "AuditLog",
                column: "At");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLog_UserId",
                schema: "dbo",
                table: "AuditLog",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicBranch_Name",
                schema: "dbo",
                table: "ClinicBranch",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diagnosis_Code",
                schema: "dbo",
                table: "Diagnosis",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_BranchId",
                schema: "dbo",
                table: "Doctor",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Doctor_SpecialtyId",
                schema: "dbo",
                table: "Doctor",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSchedule_DoctorId",
                schema: "dbo",
                table: "DoctorSchedule",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_InsurancePolicy_PatientId",
                schema: "dbo",
                table: "InsurancePolicy",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_AppointmentId",
                schema: "dbo",
                table: "Invoice",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_PatientId",
                schema: "dbo",
                table: "Invoice",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_InvoiceId",
                schema: "dbo",
                table: "InvoiceItem",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItem_ServiceId",
                schema: "dbo",
                table: "InvoiceItem",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Patient_LastName_FirstName_MiddleName",
                schema: "dbo",
                table: "Patient",
                columns: new[] { "LastName", "FirstName", "MiddleName" });

            migrationBuilder.CreateIndex(
                name: "IX_Patient_Phone",
                schema: "dbo",
                table: "Patient",
                column: "Phone");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDocument_PatientId",
                schema: "dbo",
                table: "PatientDocument",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_InvoiceId",
                schema: "dbo",
                table: "Payment",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_VisitId",
                schema: "dbo",
                table: "Prescription",
                column: "VisitId");

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItem_PriceListId_ServiceId",
                schema: "dbo",
                table: "PriceListItem",
                columns: new[] { "PriceListId", "ServiceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PriceListItem_ServiceId",
                schema: "dbo",
                table: "PriceListItem",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Role_Name",
                schema: "dbo",
                table: "Role",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_Name",
                schema: "dbo",
                table: "Service",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specialty_Name",
                schema: "dbo",
                table: "Specialty",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_User_Login",
                schema: "dbo",
                table: "User",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "dbo",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Visit_AppointmentId",
                schema: "dbo",
                table: "Visit",
                column: "AppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Visit_DoctorId",
                schema: "dbo",
                table: "Visit",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_VisitDiagnosis_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "DiagnosisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentService",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Attachment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "AuditLog",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "DoctorSchedule",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "InsurancePolicy",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "InvoiceItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PatientDocument",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Payment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Prescription",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PriceListItem",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "VisitDiagnosis",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Invoice",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "PriceList",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Service",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Diagnosis",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Visit",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Appointment",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Doctor",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Patient",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "User",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "ClinicBranch",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Specialty",
                schema: "dbo");
        }
    }
}
