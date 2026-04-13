using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitIQ.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint3_JobApplicationReasoning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GapsJson",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MatchReason",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StrengthsJson",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GapsJson",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "MatchReason",
                table: "JobApplications");

            migrationBuilder.DropColumn(
                name: "StrengthsJson",
                table: "JobApplications");
        }
    }
}
