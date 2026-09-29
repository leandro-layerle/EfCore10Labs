using Microsoft.EntityFrameworkCore;
using VectorSearch.Api.Models;

namespace VectorSearch.Api.Data;

public sealed class AppDbContext : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Document> Documents =>
        Set<Document>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Document>(
            document =>
            {
                document.HasKey(x => x.Id);

                document.Property(x => x.Title)
                    .IsRequired()
                    .HasMaxLength(200);

                document.Property(x => x.Content)
                    .IsRequired();

                document.Property(x => x.Embedding)
                    .HasColumnType("vector(3)");
            });
    }
}
