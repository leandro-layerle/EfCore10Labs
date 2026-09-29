using LeftRightJoin.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LeftRightJoin.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers =>
        Set<Customer>();

    public DbSet<Order> Orders =>
        Set<Order>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(
            customer =>
            {
                customer.HasKey(x => x.Id);

                customer.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(100);
            });

        modelBuilder.Entity<Order>(
            order =>
            {
                order.HasKey(x => x.Id);

                order.Property(x => x.Total)
                    .HasPrecision(18, 2);

                // IMPORTANTE:
                // No configuramos una FK física a propósito.
                // El laboratorio necesita una orden sin cliente coincidente
                // para demostrar claramente RightJoin.
            });
    }
}
