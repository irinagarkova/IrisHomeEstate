using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFavoriteCountSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FavoriteCount",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "a98586b5-2793-4f29-87b1-7036242f4550", "004278c0-971b-437c-97f7-a96b5336a107" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedOn", "FavoriteCount" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 18, 40, 181, DateTimeKind.Utc).AddTicks(5278), 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                column: "FavoriteCount",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedOn", "FavoriteCount" },
                values: new object[] { new DateTime(2025, 7, 25, 13, 18, 40, 181, DateTimeKind.Utc).AddTicks(5289), 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedOn", "FavoriteCount" },
                values: new object[] { new DateTime(2024, 10, 25, 16, 18, 40, 181, DateTimeKind.Local).AddTicks(5296), 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedOn", "FavoriteCount" },
                values: new object[] { new DateTime(2025, 4, 25, 16, 18, 40, 181, DateTimeKind.Local).AddTicks(5358), 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FavoriteCount",
                table: "Properties");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "af9789d6-c161-49e5-8f96-a28a90239956", "3ce8cef9-7f3f-4c1b-af7d-a242c498754e" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 24, 15, 8, 6, 501, DateTimeKind.Utc).AddTicks(2931));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 24, 15, 8, 6, 501, DateTimeKind.Utc).AddTicks(2941));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 24, 18, 8, 6, 501, DateTimeKind.Local).AddTicks(2950));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 24, 18, 8, 6, 501, DateTimeKind.Local).AddTicks(3005));
        }
    }
}
