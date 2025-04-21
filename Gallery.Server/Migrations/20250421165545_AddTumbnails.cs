using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Gallery.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTumbnails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageFilePath",
                table: "Images",
                newName: "TumbnailUrl");

            migrationBuilder.AddColumn<string>(
                name: "RealImagePath",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TumbnailPath",
                table: "Images",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RealImagePath",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "TumbnailPath",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "TumbnailUrl",
                table: "Images",
                newName: "ImageFilePath");
        }
    }
}
