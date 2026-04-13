using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecruitIQ.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4_RBAC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "JobPostings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedById",
                table: "Candidates",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "JobPostings");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Candidates");
        }
    }
}
