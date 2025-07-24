using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "af9789d6-c161-49e5-8f96-a28a90239956", "3ce8cef9-7f3f-4c1b-af7d-a242c498754e" });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "City" },
                values: new object[,]
                {
                    { 5, "", "Vidin" },
                    { 6, "", "Vratsa" },
                    { 7, "", "Gabrovo" },
                    { 8, "", "Kardzhali" },
                    { 9, "", "Kyustendil" },
                    { 10, "", "Lovech" },
                    { 11, "", "Montana" },
                    { 12, "", "Pazardzhik" },
                    { 13, "", "Pernik" },
                    { 14, "", "Pleven" },
                    { 15, "", "Veliko Tarnovo" },
                    { 16, "", "Razgrad" },
                    { 17, "", "Ruse" },
                    { 18, "", "Silistra" },
                    { 19, "", "Sliven" },
                    { 20, "", "Smolyan" },
                    { 21, "", "Blagoevgrad" },
                    { 22, "", "Stara Zagora" },
                    { 23, "", "Targovishte" },
                    { 24, "", "Haskovo" },
                    { 25, "", "Shumen" },
                    { 26, "", "Yambol" },
                    { 27, "", "Dobrich" }
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "6db57863-69a8-40e2-b290-3b8e5a138c6a", "ebfaa6cf-69a2-4d5e-971f-5f9f09ee9760" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 24, 13, 50, 29, 793, DateTimeKind.Utc).AddTicks(5061));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 24, 13, 50, 29, 793, DateTimeKind.Utc).AddTicks(5071));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 24, 16, 50, 29, 793, DateTimeKind.Local).AddTicks(5142));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 24, 16, 50, 29, 793, DateTimeKind.Local).AddTicks(5210));
        }
    }
}
