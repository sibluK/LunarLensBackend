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
        
        modelBuilder.Entity<ContentBase>()
            .Property(e => e.Type)
            .HasConversion(
                v => v.ToString(),          
                v => (ContentType)Enum.Parse(typeof(ContentType), v)); 
        
        modelBuilder.Entity<ContentBase>()
            .Property(e => e.Status)
            .HasConversion(
                v => v.ToString(),           // Convert enum to string
                v => (ContentStatus)Enum.Parse(typeof(ContentStatus), v)); 
        
        modelBuilder.Entity<ContentBase>().ToTable("ContentBases");
        modelBuilder.Entity<Article>().ToTable("Articles");
        modelBuilder.Entity<News>().ToTable("News");
        modelBuilder.Entity<Event>().ToTable("Events");
        
        /*modelBuilder.Entity<Comment>()
            .HasOne(c => c.Content) // Each Comment has one ContentBase
            .WithMany(c => c.Comments) // Each ContentBase can have many Comments
            .HasForeignKey(c => c.ContentBaseId);
        
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)          // Each Comment has one User
            .WithMany(u => u.Comments)    // Each User can have many Comments
            .HasForeignKey(c => c.UserId) // The foreign key in Comment
            .OnDelete(DeleteBehavior.Cascade); // Optional: specify delete behavior
        
        modelBuilder.Entity<ContentSection>()
            .HasOne(cs => cs.ContentBase) // Each ContentSection belongs to one ContentBase
            .WithMany(c => c.ContentSections) // Each ContentBase can have many ContentSections
            .HasForeignKey(cs => cs.ContentBaseId);
        
        modelBuilder.Entity<Category>()
            .HasMany(c => c.ContentBases) // Each Category can have many ContentBases
            .WithMany(cb => cb.Categories); // Each ContentBase can belong to many Categories
        */
    }
}