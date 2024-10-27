using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LunarLensBackend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Articles_ArticleId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Events_EventId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_News_NewsId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ArticleId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EventId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_NewsId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ArticleId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "NewsId",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ApplicationUserArticle",
                columns: table => new
                {
                    ArticlesId = table.Column<int>(type: "integer", nullable: false),
                    WritersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserArticle", x => new { x.ArticlesId, x.WritersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserArticle_Articles_ArticlesId",
                        column: x => x.ArticlesId,
                        principalTable: "Articles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserArticle_AspNetUsers_WritersId",
                        column: x => x.WritersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserEvent",
                columns: table => new
                {
                    EventsId = table.Column<int>(type: "integer", nullable: false),
                    WritersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserEvent", x => new { x.EventsId, x.WritersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserEvent_AspNetUsers_WritersId",
                        column: x => x.WritersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserEvent_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUserNews",
                columns: table => new
                {
                    NewsId = table.Column<int>(type: "integer", nullable: false),
                    WritersId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserNews", x => new { x.NewsId, x.WritersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserNews_AspNetUsers_WritersId",
                        column: x => x.WritersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserNews_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserArticle_WritersId",
                table: "ApplicationUserArticle",
                column: "WritersId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserEvent_WritersId",
                table: "ApplicationUserEvent",
                column: "WritersId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserNews_WritersId",
                table: "ApplicationUserNews",
                column: "WritersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserArticle");

            migrationBuilder.DropTable(
                name: "ApplicationUserEvent");

            migrationBuilder.DropTable(
                name: "ApplicationUserNews");

            migrationBuilder.AddColumn<int>(
                name: "ArticleId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NewsId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ArticleId",
                table: "AspNetUsers",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EventId",
                table: "AspNetUsers",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NewsId",
                table: "AspNetUsers",
                column: "NewsId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Articles_ArticleId",
                table: "AspNetUsers",
                column: "ArticleId",
                principalTable: "Articles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Events_EventId",
                table: "AspNetUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_News_NewsId",
                table: "AspNetUsers",
                column: "NewsId",
                principalTable: "News",
                principalColumn: "Id");
        }
    }
}
