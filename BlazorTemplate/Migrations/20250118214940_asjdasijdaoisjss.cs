using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorTemplate.Migrations
{
    /// <inheritdoc />
    public partial class asjdasijdaoisjss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "test",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "test",
                table: "User");
        }
    }
}
