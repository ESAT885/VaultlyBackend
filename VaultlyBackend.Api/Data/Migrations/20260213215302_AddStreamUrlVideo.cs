using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VaultlyBackend.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamUrlVideo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StreamUrl",
                table: "Videos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamUrl",
                table: "Videos");
        }
    }
}
