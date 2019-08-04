using FootballScoreAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FootballScoreAPI.Data
{
    public class FootballScoreContext : DbContext
    {
        public FootballScoreContext(DbContextOptions<FootballScoreContext> options) : base(options)
        {
        }

        public DbSet<Goal> Goals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Goal>().ToTable("Goals");
        }
    }
}

