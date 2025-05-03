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
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=TourPlanner;Username=postgres;Password=yourpassword");
        }
    }
}
