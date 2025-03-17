using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XpenseTrackerWebApp.Migrations
{
    /// <inheritdoc />
    public partial class UserIdRemoval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Settings",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Settings",
                newName: "UserId");
        }
    }
}
