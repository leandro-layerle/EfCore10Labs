using JsonColumns.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JsonColumns.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers =>
        Set<Customer>();

    public DbSet<Distributor> Distributors =>
        Set<Distributor>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCustomers(modelBuilder);
        ConfigureDistributors(modelBuilder);
    }

    private static void ConfigureCustomers(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(
            customer =>
            {
                customer.HasKey(x => x.Id);

                customer.Property(x => x.Name)
                    .IsRequired();

                customer.ComplexProperty(
                    x => x.ShippingAddress,
                    address => address.ToJson());

                customer.ComplexProperty(
                    x => x.BillingAddress,
                    address => address.ToJson());
            });
    }

    private static void ConfigureDistributors(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Distributor>(
            distributor =>
            {
                distributor.HasKey(x => x.Id);

                distributor.Property(x => x.Name)
                    .IsRequired();

                // NUEVO:
                // Una colección de Complex Types en un proveedor
                // relacional se mapea a JSON.
                distributor.ComplexCollection(
                    x => x.ShippingCenters,
                    addresses => addresses.ToJson());
            });
    }
}