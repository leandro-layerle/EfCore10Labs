using ExecuteUpdateJson.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace ExecuteUpdateJson.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Blog> Blogs =>
        Set<Blog>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Blog>(
            blog =>
            {
                blog.HasKey(x => x.Id);

                blog.Property(x => x.Name)
                    .IsRequired();

                blog.ComplexProperty(
                    x => x.Details,
                    details => details.ToJson());
            });
    }
}
