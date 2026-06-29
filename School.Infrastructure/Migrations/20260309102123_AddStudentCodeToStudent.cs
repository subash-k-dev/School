using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace School.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCodeToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rollnumber",
                table: "Students",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rollnumber",
                table: "Students");
        }
    }
}
