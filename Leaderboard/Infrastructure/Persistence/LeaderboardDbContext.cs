using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

internal class LeaderboardDbContext : DbContext
{
    public LeaderboardDbContext(DbContextOptions<LeaderboardDbContext> options) : base(options) { }

    public DbSet<LeaderboardEntry> LeaderboardEntries { get; set; }
    public DbSet<Prize> Prizes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Prize>()
            .Property(b => b.Amount)
            .HasPrecision(18, 2);
        modelBuilder.Entity<LeaderboardEntry>()
            .Property(b => b.TotalBetAmount)
            .HasPrecision(18, 2);
        base.OnModelCreating(modelBuilder);
    }
}
