using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComplexTypes.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnedCustomerComparison : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnedCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingAddress_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedCustomers", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnedCustomers");
        }
    }
}
