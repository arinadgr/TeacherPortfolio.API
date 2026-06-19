using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherPortfolio.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "documentpath",
                table: "teacher_contests",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "documentpath",
                table: "studentachievements",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "documentpath",
                table: "teacher_contests");

            migrationBuilder.DropColumn(
                name: "documentpath",
                table: "studentachievements");
        }
    }
}
