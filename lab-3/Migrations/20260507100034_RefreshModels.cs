using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lab3.Migrations
{
    /// <inheritdoc />
    public partial class RefreshModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 1, 1 },
                column: "AddedAt",
                value: new DateTime(2024, 1, 1, 10, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 1 },
                column: "AddedAt",
                value: new DateTime(2024, 1, 2, 10, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 3 },
                column: "AddedAt",
                value: new DateTime(2024, 1, 2, 10, 15, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 3, 1 },
                column: "AddedAt",
                value: new DateTime(2024, 1, 3, 10, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewedAt",
                value: new DateTime(2024, 2, 10, 14, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewedAt",
                value: new DateTime(2024, 2, 12, 16, 45, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewedAt",
                value: new DateTime(2024, 2, 15, 11, 20, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "JoinedAt",
                value: new DateTime(2023, 1, 15, 8, 30, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "JoinedAt",
                value: new DateTime(2023, 2, 20, 9, 15, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "JoinedAt",
                value: new DateTime(2023, 3, 10, 7, 45, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 1, 1 },
                column: "AddedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 596, DateTimeKind.Local).AddTicks(6281));

            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 1 },
                column: "AddedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 598, DateTimeKind.Local).AddTicks(9416));

            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 2, 3 },
                column: "AddedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 598, DateTimeKind.Local).AddTicks(9435));

            migrationBuilder.UpdateData(
                table: "BookGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { 3, 1 },
                column: "AddedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 598, DateTimeKind.Local).AddTicks(9438));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1,
                column: "ReviewedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(2571));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2,
                column: "ReviewedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(3278));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3,
                column: "ReviewedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(3286));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "JoinedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(984));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "JoinedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(1528));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "JoinedAt",
                value: new DateTime(2026, 5, 3, 23, 46, 45, 599, DateTimeKind.Local).AddTicks(1536));
        }
    }
}
