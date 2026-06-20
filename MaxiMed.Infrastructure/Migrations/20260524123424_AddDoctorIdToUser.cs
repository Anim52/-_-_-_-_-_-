using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MaxiMed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDoctorIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DoctorId",
                schema: "dbo",
                table: "User",
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
                name: "IX_User_DoctorId",
                schema: "dbo",
                table: "User",
                column: "DoctorId");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Doctor_DoctorId",
                schema: "dbo",
                table: "User",
                column: "DoctorId",
                principalSchema: "dbo",
                principalTable: "Doctor",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Doctor_DoctorId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropIndex(
                name: "IX_User_DoctorId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "DoctorId",
                schema: "dbo",
                table: "User");

            migrationBuilder.DropColumn(
                name: "WorkShift",
                schema: "dbo",
                table: "Doctor");
        }
    }
}
