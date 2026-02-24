using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentApp.Migrations
{
    /// <inheritdoc />
    public partial class AddFilePathsToComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Comments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextFilePath",
                table: "Comments",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TextFilePath",
                table: "Comments");
        }
    }
}
