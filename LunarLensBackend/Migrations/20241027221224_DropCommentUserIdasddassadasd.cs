using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunarLensBackend.Migrations
{
    /// <inheritdoc />
    public partial class DropCommentUserIdasddassadasd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Comments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Comments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
