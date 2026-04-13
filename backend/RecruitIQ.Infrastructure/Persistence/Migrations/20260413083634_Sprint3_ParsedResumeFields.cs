using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitIQ.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_ParsedResumeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParsedEducationJson",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParsedExperienceJson",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParsedSkillsJson",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParsedSummary",
                table: "Candidates",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParsedEducationJson",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ParsedExperienceJson",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ParsedSkillsJson",
                table: "Candidates");

            migrationBuilder.DropColumn(
                name: "ParsedSummary",
                table: "Candidates");
        }
    }
}
