using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitIQ.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4_KanbanStage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Stage",
                table: "JobApplications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stage",
                table: "JobApplications");
        }
    }
}
