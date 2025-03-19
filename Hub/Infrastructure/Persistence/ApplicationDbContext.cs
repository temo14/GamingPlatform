using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Bet> Bets { get; set; }
    public DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bet>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);
        base.OnModelCreating(modelBuilder);
    }
}
