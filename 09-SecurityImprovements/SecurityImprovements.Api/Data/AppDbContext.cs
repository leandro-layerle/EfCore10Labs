using Microsoft.EntityFrameworkCore;
using SecurityImprovements.Api.Models;

namespace SecurityImprovements.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users =>
        Set<User>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(
            user =>
            {
                user.HasKey(x => x.Id);

                user.Property(x => x.DisplayName)
                    .IsRequired()
                    .HasMaxLength(120);

                user.Property(x => x.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                user.Property(x => x.Role)
                    .IsRequired()
                    .HasMaxLength(80);
            });
    }
}
