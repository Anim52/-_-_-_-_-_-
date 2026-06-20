using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaxiMed.Infrastructure.Migrations
{
    public partial class AddDoctorUserLinkAndWorkShift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                schema: "dbo",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkShift",
                schema: "dbo",
                table: "Doctor",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "AllDay");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DoctorId",
                schema: "dbo",
                table: "Users",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Doctor_DoctorId",
                schema: "dbo",
                table: "Users",
                column: "DoctorId",
                principalSchema: "dbo",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Doctor_DoctorId",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DoctorId",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                schema: "dbo",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WorkShift",
                schema: "dbo",
                table: "Doctor");
        }
    }
}
