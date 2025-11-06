using chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ChatMessageEntity> ChatMessages { get; set; }
    public DbSet<EmbeddingData> EmbeddingData { get; set; }
    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<EmbeddingData>()
            .Property(e => e.Vector)
            .HasColumnType("vector(768)");
    }
}
