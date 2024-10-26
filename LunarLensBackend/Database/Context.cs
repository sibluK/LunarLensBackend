using LunarLensBackend.Entities;
using LunarLensBackend.Entities.Enums;
using LunarLensBackend.Entities.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LunarLensBackend.Database;

public class Context : IdentityDbContext<ApplicationUser, IdentityRole, string>
{
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<News> News { get; set; }
    public DbSet<Article> Articles { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Category> Categories { get; set; }
    public Context(DbContextOptions<Context> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RefreshToken>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(rt => rt.UserId);
        
        modelBuilder.Entity<News>()
            .Property(n => n.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ContentStatus)Enum.Parse(typeof(ContentStatus), v));

        // Configure Article entity
        modelBuilder.Entity<Article>()
            .Property(a => a.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ContentStatus)Enum.Parse(typeof(ContentStatus), v));

        // Configure Event entity
        modelBuilder.Entity<Event>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),
                v => (ContentStatus)Enum.Parse(typeof(ContentStatus), v));
    }
}