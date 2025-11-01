using chatbot.Models;
using Microsoft.EntityFrameworkCore;

namespace chatbot.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ChatMessageEntity> ChatMessages { get; set; }
}
