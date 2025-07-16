using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "User", "USER" },
                    { "2", null, "Admin", "ADMIN" }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "0dc71052-6160-46a2-a19b-0ff1c7a66576", "2a573539-5908-4769-9086-10f3b20eb2ef" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 16, 11, 17, 14, 204, DateTimeKind.Utc).AddTicks(1230));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 16, 11, 17, 14, 204, DateTimeKind.Utc).AddTicks(1238));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 16, 14, 17, 14, 204, DateTimeKind.Local).AddTicks(1245));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 16, 14, 17, 14, 204, DateTimeKind.Local).AddTicks(1310));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "7a184ae1-cfc3-4a49-8de3-e65665203d3a", "9d4fcad3-c172-46fd-a149-d156af3065c0" });

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
    }
}
