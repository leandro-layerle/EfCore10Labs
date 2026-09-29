using ComplexTypes.Api.Models;
using ComplexTypes.Api.Models.Owned;
using Microsoft.EntityFrameworkCore;

namespace ComplexTypes.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers =>
        Set<Customer>();

    public DbSet<OwnedCustomer> OwnedCustomers =>
        Set<OwnedCustomer>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureComplexTypes(modelBuilder);

        ConfigureOwnedTypes(modelBuilder);
    }

    private static void ConfigureComplexTypes(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(
            customer =>
            {
                customer.HasKey(x => x.Id);

                customer.Property(x => x.Name)
                    .IsRequired();

                customer.ComplexProperty(
                    x => x.ShippingAddress);

                customer.ComplexProperty(
                    x => x.BillingAddress);
            });
    }

    private static void ConfigureOwnedTypes(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OwnedCustomer>(
            customer =>
            {
                customer.HasKey(x => x.Id);

                customer.Property(x => x.Name)
                    .IsRequired();

                customer.OwnsOne(
                    x => x.ShippingAddress);

                customer.OwnsOne(
                    x => x.BillingAddress);
            });
    }
}