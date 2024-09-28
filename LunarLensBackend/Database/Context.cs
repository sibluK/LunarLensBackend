using Microsoft.EntityFrameworkCore;

namespace LunarLensBackend.Database;

public class Context : DbContext
{
    
    public DbSet<AppUser> Users { get; set; }
    
    public Context(DbContextOptions<Context> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

public class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}