using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NamedQueryFilters.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "TenantId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Order 1 - Tenant 1 - Active", false, 1 },
                    { 2, new DateTime(2026, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Order 2 - Tenant 1 - Deleted", true, 1 },
                    { 3, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Order 3 - Tenant 2 - Active", false, 2 },
                    { 4, new DateTime(2026, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Order 4 - Tenant 2 - Deleted", true, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Orders",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
