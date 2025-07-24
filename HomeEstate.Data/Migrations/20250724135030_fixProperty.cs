using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsParking",
                table: "Properties",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyType",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                columns: new[] { "CreatedOn", "IsParking", "PropertyType" },
                values: new object[] { new DateTime(2025, 7, 24, 13, 50, 29, 793, DateTimeKind.Utc).AddTicks(5061), null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IsParking", "PropertyType" },
                values: new object[] { null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedOn", "IsParking", "PropertyType" },
                values: new object[] { new DateTime(2025, 7, 24, 13, 50, 29, 793, DateTimeKind.Utc).AddTicks(5071), null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedOn", "IsParking", "PropertyType" },
                values: new object[] { new DateTime(2024, 10, 24, 16, 50, 29, 793, DateTimeKind.Local).AddTicks(5142), null, 0 });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedOn", "IsParking", "PropertyType" },
                values: new object[] { new DateTime(2025, 4, 24, 16, 50, 29, 793, DateTimeKind.Local).AddTicks(5210), null, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsParking",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Properties");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "3538844c-61d7-4b42-b608-dc1945f65a9c", "40cb2b2d-ba3a-4069-9a24-ed483a9c90ae" });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 24, 13, 4, 3, 758, DateTimeKind.Utc).AddTicks(6464));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedOn",
                value: new DateTime(2025, 7, 24, 13, 4, 3, 758, DateTimeKind.Utc).AddTicks(6472));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedOn",
                value: new DateTime(2024, 10, 24, 16, 4, 3, 758, DateTimeKind.Local).AddTicks(6478));

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedOn",
                value: new DateTime(2025, 4, 24, 16, 4, 3, 758, DateTimeKind.Local).AddTicks(6533));
        }
    }
}
