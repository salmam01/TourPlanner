using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.DataLayer.Data
{
    public class TourPlannerDbContext : DbContext
    {
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourDetails> TourDetails { get; set; }
        public DbSet<TourAttributes> TourAttributes { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        //  Set the connection string
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //  TODO: Safely store the variables for the connection string
            string host = "localhost";
            string port = "5432";
            string database = "TourPlanner";
            string username = "salma";
            string password = "tourPlanner1234";

            optionsBuilder.UseNpgsql($"Host={host};Port={port};Database={database};Username={username};Password={password}");
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
