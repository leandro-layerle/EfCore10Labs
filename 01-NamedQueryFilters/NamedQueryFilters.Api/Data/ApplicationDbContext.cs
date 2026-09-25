using Microsoft.EntityFrameworkCore;
using NamedQueryFilters.Api.Entities;

namespace NamedQueryFilters.Api.Data;

public class ApplicationDbContext : DbContext
{
    public int CurrentTenantId { get; } = 1;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>()
            .HasQueryFilter(
                QueryFilterNames.SoftDeletion,
                order => !order.IsDeleted)
            .HasQueryFilter(
                QueryFilterNames.Tenant,
                order => order.TenantId == CurrentTenantId);

        modelBuilder.Entity<Order>().HasData(
            new Order
            {
                Id = 1,
                Description = "Order 1 - Tenant 1 - Active",
                TenantId = 1,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 10)
            },
            new Order
            {
                Id = 2,
                Description = "Order 2 - Tenant 1 - Deleted",
                TenantId = 1,
                IsDeleted = true,
                CreatedAt = new DateTime(2026, 1, 11)
            },
            new Order
            {
                Id = 3,
                Description = "Order 3 - Tenant 2 - Active",
                TenantId = 2,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 12)
            },
            new Order
            {
                Id = 4,
                Description = "Order 4 - Tenant 2 - Deleted",
                TenantId = 2,
                IsDeleted = true,
                CreatedAt = new DateTime(2026, 1, 13)
            });
    }
}