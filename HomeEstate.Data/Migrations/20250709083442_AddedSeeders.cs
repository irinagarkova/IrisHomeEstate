using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HomeEstate.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedSeeders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfilePictureURL", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "3da54138-8954-47bf-9d6f-e4d4643bd2da", 0, "8d80ea8d-843c-41c8-ab31-a01634b3b12e", "nqkuv@email.com", false, false, null, null, null, null, null, false, null, "0832ca7c-452b-4879-976e-c4f33740da7d", false, null });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Apartment" },
                    { 2, "House" },
                    { 3, "Office" },
                    { 4, "Villa" }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "City" },
                values: new object[,]
                {
                    { 1, "123 Vitosha Blvd", "Sofia" },
                    { 2, "45 Kapana Street", "Plovdiv" },
                    { 3, "10 Sea Garden Ave", "Varna" },
                    { 4, "78 Central Square", "Burgas" }
                });

            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "Area", "CategoryId", "CreatedOn", "Description", "LocationId", "OwnerId", "Price", "Title" },
                values: new object[,]
                {
                    { 1, 120, 1, new DateTime(2025, 7, 9, 8, 34, 42, 540, DateTimeKind.Utc).AddTicks(5770), "Modern apartment located in the city center with great amenities.", 1, "3da54138-8954-47bf-9d6f-e4d4643bd2da", 250000m, "Luxury Apartment in Sofia" },
                    { 2, 200, 2, new DateTime(2025, 7, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), "Beautiful countryside house with a big garden.", 2, "3da54138-8954-47bf-9d6f-e4d4643bd2da", 180000m, "Cozy House in the Countryside" },
                    { 3, 180, 3, new DateTime(2025, 7, 9, 8, 34, 42, 540, DateTimeKind.Utc).AddTicks(5780), "Prime commercial location with modern infrastructure and ample parking.", 3, "3da54138-8954-47bf-9d6f-e4d4643bd2da", 16000m, "Commercial Office Space" },
                    { 4, 800, 1, new DateTime(2024, 10, 9, 11, 34, 42, 540, DateTimeKind.Local).AddTicks(5786), "High-end condo with premium amenities, pool access, and concierge services.", 1, "3da54138-8954-47bf-9d6f-e4d4643bd2da", 27500000m, "Luxury Apartment with Pool" },
                    { 5, 2500, 4, new DateTime(2025, 4, 9, 11, 34, 42, 540, DateTimeKind.Local).AddTicks(5845), "Perfect family home with large backyard, garage, and excellent school district.", 4, "3da54138-8954-47bf-9d6f-e4d4643bd2da", 450000.00m, "Spacious Family House" }
                });

            migrationBuilder.InsertData(
                table: "PropertyImages",
                columns: new[] { "Id", "ImageUrl", "PropertyId" },
                values: new object[,]
                {
                    { 1, "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800&h=600&fit=crop", 1 },
                    { 2, "https://images.unsplash.com/photo-1586023492125-27b2c045efd7?w=800&h=600&fit=crop", 1 },
                    { 3, "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=800&h=600&fit=crop", 1 },
                    { 4, "https://images.unsplash.com/photo-1518780664697-55e3ad937233?w=800&h=600&fit=crop", 2 },
                    { 5, "https://images.unsplash.com/photo-1560448204-e02f11c3d0e2?w=800&h=600&fit=crop", 2 },
                    { 6, "https://images.unsplash.com/photo-1505693314120-0d443867891c?w=800&h=600&fit=crop", 2 },
                    { 7, "https://images.unsplash.com/photo-1552321554-5fefe8c9ef14?w=800&h=600&fit=crop", 2 },
                    { 8, "https://images.unsplash.com/photo-1497366216548-37526070297c?w=800&h=600&fit=crop", 3 },
                    { 9, "https://images.unsplash.com/photo-1497366811353-6870744d04b2?w=800&h=600&fit=crop", 3 },
                    { 10, "https://img.vila.bg/g/6556/170637.jpg", 4 },
                    { 11, "https://api.photon.aremedia.net.au/wp-content/uploads/sites/2/umb-media/25922/resort-style-1980s-home-renovation-living-room-vaulted-a-frame-ceiling.jpg?crop=0px%2C1001px%2C1467px%2C825px&resize=720%2C405", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Properties_LocationId",
                table: "Properties",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Properties_Locations_LocationId",
                table: "Properties",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Properties_Locations_LocationId",
                table: "Properties");

            migrationBuilder.DropIndex(
                name: "IX_Properties_LocationId",
                table: "Properties");

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "PropertyImages",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3da54138-8954-47bf-9d6f-e4d4643bd2da");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Locations",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Properties");
        }
    }
}
