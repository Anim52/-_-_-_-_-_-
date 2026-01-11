using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaxiMed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientSnils : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitDiagnosis_Diagnoses_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis");

            migrationBuilder.AddColumn<string>(
                name: "Snils",
                schema: "dbo",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDiagnosis_Diagnoses_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "DiagnosisId",
                principalSchema: "dbo",
                principalTable: "Diagnoses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitDiagnosis_Diagnoses_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis");

            migrationBuilder.DropColumn(
                name: "Snils",
                schema: "dbo",
                table: "Patient");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitDiagnosis_Diagnoses_DiagnosisId",
                schema: "dbo",
                table: "VisitDiagnosis",
                column: "DiagnosisId",
                principalSchema: "dbo",
                principalTable: "Diagnoses",
                principalColumn: "Id");
        }
    }
}
