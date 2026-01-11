using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaxiMed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisitsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Visit_VisitId",
                schema: "dbo",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescription_Visit_VisitId",
                schema: "dbo",
                table: "Prescription");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Appointment_AppointmentId",
                schema: "dbo",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_Visit_Doctor_DoctorId",
                schema: "dbo",
                table: "Visit");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitDiagnosis_Diagnosis_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitDiagnosis_Visit_VisitId",
                schema: "dbo",
                table: "VisitDiagnosis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visit",
                schema: "dbo",
                table: "Visit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescription",
                schema: "dbo",
                table: "Prescription");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagnosis",
                schema: "dbo",
                table: "Diagnosis");

            migrationBuilder.RenameTable(
                name: "Visit",
                schema: "dbo",
                newName: "Visits",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Prescription",
                schema: "dbo",
                newName: "Prescriptions",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Diagnosis",
                schema: "dbo",
                newName: "Diagnoses",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_DoctorId",
                schema: "dbo",
                table: "Visits",
                newName: "IX_Visits_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Visit_AppointmentId",
                schema: "dbo",
                table: "Visits",
                newName: "IX_Visits_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescription_VisitId",
                schema: "dbo",
                table: "Prescriptions",
                newName: "IX_Prescriptions_VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnosis_Code",
                schema: "dbo",
                table: "Diagnoses",
                newName: "IX_Diagnoses_Code");

            migrationBuilder.AddColumn<int>(
                name: "DoctorId1",
                schema: "dbo",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Diagnoses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "Diagnoses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visits",
                schema: "dbo",
                table: "Visits",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescriptions",
                schema: "dbo",
                table: "Prescriptions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagnoses",
                schema: "dbo",
                table: "Diagnoses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Visits_DoctorId1",
                schema: "dbo",
                table: "Visits",
                column: "DoctorId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Visits_VisitId",
                schema: "dbo",
                table: "Attachment",
                column: "VisitId",
                principalSchema: "dbo",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescriptions_Visits_VisitId",
                schema: "dbo",
                table: "Prescriptions",
                column: "VisitId",
                principalSchema: "dbo",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDiagnosis_Diagnoses_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "DiagnosisId",
                principalSchema: "dbo",
                principalTable: "Diagnoses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDiagnosis_Visits_VisitId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "VisitId",
                principalSchema: "dbo",
                principalTable: "Visits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Appointment_AppointmentId",
                schema: "dbo",
                table: "Visits",
                column: "AppointmentId",
                principalSchema: "dbo",
                principalTable: "Appointment",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Doctor_DoctorId",
                schema: "dbo",
                table: "Visits",
                column: "DoctorId",
                principalSchema: "dbo",
                principalTable: "Doctor",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Doctor_DoctorId1",
                schema: "dbo",
                table: "Visits",
                column: "DoctorId1",
                principalSchema: "dbo",
                principalTable: "Doctor",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_Visits_VisitId",
                schema: "dbo",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Prescriptions_Visits_VisitId",
                schema: "dbo",
                table: "Prescriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitDiagnosis_Diagnoses_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis");

            migrationBuilder.DropForeignKey(
                name: "FK_VisitDiagnosis_Visits_VisitId",
                schema: "dbo",
                table: "VisitDiagnosis");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Appointment_AppointmentId",
                schema: "dbo",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Doctor_DoctorId",
                schema: "dbo",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Doctor_DoctorId1",
                schema: "dbo",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Visits",
                schema: "dbo",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_DoctorId1",
                schema: "dbo",
                table: "Visits");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prescriptions",
                schema: "dbo",
                table: "Prescriptions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Diagnoses",
                schema: "dbo",
                table: "Diagnoses");

            migrationBuilder.DropColumn(
                name: "DoctorId1",
                schema: "dbo",
                table: "Visits");

            migrationBuilder.RenameTable(
                name: "Visits",
                schema: "dbo",
                newName: "Visit",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Prescriptions",
                schema: "dbo",
                newName: "Prescription",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Diagnoses",
                schema: "dbo",
                newName: "Diagnosis",
                newSchema: "dbo");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_DoctorId",
                schema: "dbo",
                table: "Visit",
                newName: "IX_Visit_DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_AppointmentId",
                schema: "dbo",
                table: "Visit",
                newName: "IX_Visit_AppointmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Prescriptions_VisitId",
                schema: "dbo",
                table: "Prescription",
                newName: "IX_Prescription_VisitId");

            migrationBuilder.RenameIndex(
                name: "IX_Diagnoses_Code",
                schema: "dbo",
                table: "Diagnosis",
                newName: "IX_Diagnosis_Code");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Diagnosis",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "dbo",
                table: "Diagnosis",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Visit",
                schema: "dbo",
                table: "Visit",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prescription",
                schema: "dbo",
                table: "Prescription",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Diagnosis",
                schema: "dbo",
                table: "Diagnosis",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_Visit_VisitId",
                schema: "dbo",
                table: "Attachment",
                column: "VisitId",
                principalSchema: "dbo",
                principalTable: "Visit",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Prescription_Visit_VisitId",
                schema: "dbo",
                table: "Prescription",
                column: "VisitId",
                principalSchema: "dbo",
                principalTable: "Visit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Appointment_AppointmentId",
                schema: "dbo",
                table: "Visit",
                column: "AppointmentId",
                principalSchema: "dbo",
                principalTable: "Appointment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Visit_Doctor_DoctorId",
                schema: "dbo",
                table: "Visit",
                column: "DoctorId",
                principalSchema: "dbo",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDiagnosis_Diagnosis_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "DiagnosisId",
                principalSchema: "dbo",
                principalTable: "Diagnosis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDiagnosis_Visit_VisitId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "VisitId",
                principalSchema: "dbo",
                principalTable: "Visit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
