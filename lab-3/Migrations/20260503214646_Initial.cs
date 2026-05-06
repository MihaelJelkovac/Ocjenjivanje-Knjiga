using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Lab3.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Biography = table.Column<string>(type: "TEXT", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Nationality = table.Column<string>(type: "TEXT", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Audience = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Publishers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    City = table.Column<string>(type: "TEXT", nullable: false),
                    Country = table.Column<string>(type: "TEXT", nullable: false),
                    FoundedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    FullName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FavoriteGenre = table.Column<string>(type: "TEXT", nullable: false),
                    ReputationPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPremiumMember = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Books",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Isbn = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PublishedOn = table.Column<DateTime>(type: "TEXT", nullable: false),
                    PageCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Language = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    PublisherId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Books", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Books_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Books_Publishers_PublisherId",
                        column: x => x.PublisherId,
                        principalTable: "Publishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookGenres",
                columns: table => new
                {
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookGenres", x => new { x.BookId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_BookGenres_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsRecommended = table.Column<bool>(type: "INTEGER", nullable: false),
                    Sentiment = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Biography", "BirthDate", "FirstName", "LastName", "Nationality", "Website" },
                values: new object[,]
                {
                    { 1, "British author", new DateTime(1965, 7, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "J.K.", "Rowling", "British", "https://www.jkrowling.com" },
                    { 2, "American author", new DateTime(1948, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "George", "R. R. Martin", "American", "https://www.georgerrmartin.com" },
                    { 3, "English author", new DateTime(1892, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "J.R.R.", "Tolkien", "English", "https://www.tolkien.co.uk" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Audience", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Young Adults", "Fantastical worlds and magic", "Fantasy" },
                    { 2, "Adults", "Future and space", "Science Fiction" },
                    { 3, "Adults", "Human conflicts", "Drama" }
                });

            migrationBuilder.InsertData(
                table: "Publishers",
                columns: new[] { "Id", "City", "ContactEmail", "Country", "FoundedOn", "Name", "Website" },
                values: new object[,]
                {
                    { 1, "London", "info@bloomsbury.com", "UK", new DateTime(1986, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bloomsbury", "https://www.bloomsbury.com" },
                    { 2, "New York", "info@bantambooks.com", "USA", new DateTime(1945, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bantam Books", "https://www.bantambooks.com" },
                    { 3, "London", "info@allenandunwin.com", "UK", new DateTime(1914, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Allen & Unwin", "https://www.allenandunwin.com" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FavoriteGenre", "FullName", "IsPremiumMember", "JoinedAt", "ReputationPoints", "Username" },
                values: new object[,]
                {
                    { 1, "alice@example.com", "Fantasy", "Alice Reader", true, new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(984), 150, "reader1" },
                    { 2, "bob@example.com", "Science Fiction", "Bob Smith", false, new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(1528), 80, "reader2" },
                    { 3, "carol@example.com", "Fantasy", "Carol White", true, new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(1536), 200, "reader3" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "AuthorId", "Description", "Isbn", "Language", "PageCount", "PublishedOn", "PublisherId", "Status", "Title" },
                values: new object[,]
                {
                    { 1, 1, "A young wizard's journey", "978-0-7475-3269-9", "English", 309, new DateTime(1997, 6, 26, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, "Harry Potter and the Philosopher's Stone" },
                    { 2, 2, "Epic fantasy drama", "978-0-553-10354-1", "English", 694, new DateTime(1996, 8, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, "A Game of Thrones" },
                    { 3, 3, "Epic fantasy adventure", "978-0-544-00362-6", "English", 423, new DateTime(1954, 7, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, "The Fellowship of the Ring" }
                });

            migrationBuilder.InsertData(
                table: "BookGenres",
                columns: new[] { "BookId", "GenreId", "AddedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 3, 23, 46, 45, 596, DateTimeKind.Local).AddTicks(6281) },
                    { 2, 1, new DateTime(2026, 5, 3, 23, 46, 45, 598, DateTimeKind.Local).AddTicks(9416) },
                    { 2, 3, new DateTime(2026, 5, 3, 23, 46, 45, 598, DateTimeKind.Local).AddTicks(9435) },
                    { 3, 1, new DateTime(2026, 5, 3, 23, 46, 45, 598, DateTimeKind.Local).AddTicks(9438) }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "BookId", "Comment", "IsRecommended", "ReviewedAt", "Score", "Sentiment", "Title", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "Best book ever", true, new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(2571), 5, 3, "Amazing!", 1 },
                    { 2, 2, "Very engaging", true, new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(3278), 4, 2, "Great read", 2 },
                    { 3, 3, "A masterpiece", true, new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(3286), 5, 3, "Epic!", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookGenres_GenreId",
                table: "BookGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_PublisherId",
                table: "Books",
                column: "PublisherId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookId",
                table: "Reviews",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookGenres");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Books");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.DropTable(
                name: "Publishers");
        }
    }
}
