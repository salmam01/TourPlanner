using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.Configuration;

namespace TourPlanner.DataLayer.Data
{
    public class TourPlannerDbContext : DbContext
    {
        private readonly DatabaseConfig _databaseConfig;
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourDetails> TourDetails { get; set; }
        public DbSet<TourAttributes> TourAttributes { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options, DatabaseConfig databaseConfig)
            : base(options)
        {
            _databaseConfig = databaseConfig;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(
                $"Host={_databaseConfig.Host};Port={_databaseConfig.Port};Database={_databaseConfig.Database};Username={_databaseConfig.Username};Password={_databaseConfig.Password}"
            );
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  One tour has one tour details
            modelBuilder.Entity<Tour>()
                .HasOne(t => t.TourDetails)
                .WithOne()
                .HasForeignKey<TourDetails>(td => td.Id);

            //  One tour has one tour attributes
            modelBuilder.Entity<Tour>()
                .HasOne(t => t.TourAttributes)
                .WithOne()
                .HasForeignKey<TourAttributes>(ta => ta.Id);

            //  One tour has many tour logs
            modelBuilder.Entity<Tour>()
                .HasMany(t => t.TourLogs)
                .WithOne(tl => tl.Tour)
                .HasForeignKey(tl => tl.TourId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
