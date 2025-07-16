using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_FavPropSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "7a184ae1-cfc3-4a49-8de3-e65665203d3a", "9d4fcad3-c172-46fd-a149-d156af3065c0" });

            migrationBuilder.InsertData(
                table: "FavoriteProperties",
                columns: new[] { "PropertyId", "UserId" },
                values: new object[,]
                {
                    { 1, "3da54138-8954-47bf-9d6f-e4d4643bd2da" },
                    { 2, "3da54138-8954-47bf-9d6f-e4d4643bd2da" },
                    { 3, "3da54138-8954-47bf-9d6f-e4d4643bd2da" }
                });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 16, 10, 1, 45, 377, DateTimeKind.Utc).AddTicks(7499));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 16, 10, 1, 45, 377, DateTimeKind.Utc).AddTicks(7508));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 16, 13, 1, 45, 377, DateTimeKind.Local).AddTicks(7549));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 16, 13, 1, 45, 377, DateTimeKind.Local).AddTicks(7604));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "FavoriteProperties",
                keyColumns: new[] { "PropertyId", "UserId" },
                keyValues: new object[] { 1, "3da54138-8954-47bf-9d6f-e4d4643bd2da" });

            migrationBuilder.DeleteData(
                table: "FavoriteProperties",
                keyColumns: new[] { "PropertyId", "UserId" },
                keyValues: new object[] { 2, "3da54138-8954-47bf-9d6f-e4d4643bd2da" });

            migrationBuilder.DeleteData(
                table: "FavoriteProperties",
                keyColumns: new[] { "PropertyId", "UserId" },
                keyValues: new object[] { 3, "3da54138-8954-47bf-9d6f-e4d4643bd2da" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "8d80ea8d-843c-41c8-ab31-a01634b3b12e", "0832ca7c-452b-4879-976e-c4f33740da7d" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 9, 8, 34, 42, 540, DateTimeKind.Utc).AddTicks(5770));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 9, 8, 34, 42, 540, DateTimeKind.Utc).AddTicks(5780));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 9, 11, 34, 42, 540, DateTimeKind.Local).AddTicks(5786));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 9, 11, 34, 42, 540, DateTimeKind.Local).AddTicks(5845));
        }
    }
}
