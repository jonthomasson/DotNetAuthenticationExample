using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetAuthentication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserCreatedDateTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedTimestamp",
                table: "Users",
                newName: "CreatedDateTimestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreatedDateTimestamp",
                table: "Users",
                newName: "CreatedTimestamp");
        }
    }
}
