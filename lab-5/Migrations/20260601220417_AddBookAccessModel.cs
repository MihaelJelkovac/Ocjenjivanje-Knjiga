using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab5.Migrations
{
    /// <inheritdoc />
    public partial class AddBookAccessModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    AccessLevel = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    GrantedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Reason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookAccesses_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAccesses_AspNetUsers_GrantedBy",
                        column: x => x.GrantedBy,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BookAccesses_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookAccesses_AppUserId",
                table: "BookAccesses",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookAccesses_BookId",
                table: "BookAccesses",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookAccesses_GrantedBy",
                table: "BookAccesses",
                column: "GrantedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAccesses");
        }
    }
}
