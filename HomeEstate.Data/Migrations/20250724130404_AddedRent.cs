using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedRent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AvailableFrom",
                table: "Properties",
                type: "datetime2",
                nullable: true,
                comment: "Available from date");

            migrationBuilder.AddColumn<bool>(
                name: "IsFurnished",
                table: "Properties",
                type: "bit",
                nullable: true,
                comment: "Indicates if property is furnished");

            migrationBuilder.AddColumn<int>(
                name: "ListingType",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Property Listing Type - Sale, Rent or Both");

            migrationBuilder.AddColumn<int>(
                name: "MinimumLeasePeriod",
                table: "Properties",
                type: "int",
                nullable: true,
                comment: "Minimum lease period in months");

            migrationBuilder.AddColumn<decimal>(
                name: "MonthlyRent",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Monthly rent if property is for rent");

            migrationBuilder.AddColumn<bool>(
                name: "PetsAllowed",
                table: "Properties",
                type: "bit",
                nullable: true,
                comment: "Indicates if pets are allowed for rentals");

            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDeposit",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Security deposit for rental properties");

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
                columns: new[] { "AvailableFrom", "CreatedOn", "IsFurnished", "ListingType", "MinimumLeasePeriod", "MonthlyRent", "PetsAllowed", "SecurityDeposit" },
                values: new object[] { null, new DateTime(2025, 7, 24, 13, 4, 3, 758, DateTimeKind.Utc).AddTicks(6464), null, 1, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AvailableFrom", "IsFurnished", "ListingType", "MinimumLeasePeriod", "MonthlyRent", "PetsAllowed", "SecurityDeposit" },
                values: new object[] { null, null, 1, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AvailableFrom", "CreatedOn", "IsFurnished", "ListingType", "MinimumLeasePeriod", "MonthlyRent", "PetsAllowed", "SecurityDeposit" },
                values: new object[] { null, new DateTime(2025, 7, 24, 13, 4, 3, 758, DateTimeKind.Utc).AddTicks(6472), null, 1, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AvailableFrom", "CreatedOn", "IsFurnished", "ListingType", "MinimumLeasePeriod", "MonthlyRent", "PetsAllowed", "SecurityDeposit" },
                values: new object[] { null, new DateTime(2024, 10, 24, 16, 4, 3, 758, DateTimeKind.Local).AddTicks(6478), null, 1, null, null, null, null });

            migrationBuilder.UpdateData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AvailableFrom", "CreatedOn", "IsFurnished", "ListingType", "MinimumLeasePeriod", "MonthlyRent", "PetsAllowed", "SecurityDeposit" },
                values: new object[] { null, new DateTime(2025, 4, 24, 16, 4, 3, 758, DateTimeKind.Local).AddTicks(6533), null, 1, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableFrom",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsFurnished",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ListingType",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MinimumLeasePeriod",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "MonthlyRent",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PetsAllowed",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SecurityDeposit",
                table: "Properties");

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
    }
}
