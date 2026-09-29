using DynamicExecuteUpdate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DynamicExecuteUpdate.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(product => product.Name)
                .HasMaxLength(200);

            entity.Property(product => product.Price)
                .HasPrecision(18, 2);
        });
    }
}
