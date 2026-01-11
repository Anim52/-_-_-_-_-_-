using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaxiMed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientOmsPassport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OmsPolicyNumber",
                schema: "dbo",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassportNumber",
                schema: "dbo",
                table: "Patient",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OmsPolicyNumber",
                schema: "dbo",
                table: "Patient");

            migrationBuilder.DropColumn(
                name: "PassportNumber",
                schema: "dbo",
                table: "Patient");
        }
    }
}
