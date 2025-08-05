using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class Test_Delete_Images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "ddbb3fc6-8e89-45a6-b356-9e3332fc3c4a", "14214ee6-24de-4b9e-8472-3a42fb73ecfb" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 1, 14, 24, 39, 84, DateTimeKind.Utc).AddTicks(6785));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 8, 1, 14, 24, 39, 84, DateTimeKind.Utc).AddTicks(6796));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 11, 1, 17, 24, 39, 84, DateTimeKind.Local).AddTicks(6804));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 5, 1, 17, 24, 39, 84, DateTimeKind.Local).AddTicks(6870));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                column: "CreatedOn",
                value: new DateTime(2025, 7, 25, 13, 18, 40, 181, DateTimeKind.Utc).AddTicks(5278));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 25, 13, 18, 40, 181, DateTimeKind.Utc).AddTicks(5289));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 25, 16, 18, 40, 181, DateTimeKind.Local).AddTicks(5296));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 25, 16, 18, 40, 181, DateTimeKind.Local).AddTicks(5358));
        }
    }
}
