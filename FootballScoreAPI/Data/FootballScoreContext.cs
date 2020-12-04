using FootballScoreAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FootballScoreAPI.Data
{
    public class FootballScoreContext : DbContext
    {
        public FootballScoreContext(DbContextOptions<FootballScoreContext> options) : base(options)
        {
        }

        public DbSet<Fixture> Fixtures { get; set; }
        public DbSet<Goal> Goals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fixture>().ToTable("Fixtures");
            modelBuilder.Entity<Goal>().ToTable("Goals");
        }
    }
}

