using Microsoft.EntityFrameworkCore;
using ParameterizedCollections.Api.Models;

namespace ParameterizedCollections.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products =>
        Set<Product>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(
            product =>
            {
                product.HasKey(x => x.Id);

                product.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(120);

                product.Property(x => x.Price)
                    .HasPrecision(18, 2);
            });
    }
}
