using Microsoft.EntityFrameworkCore;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace TourPlanner.DAL.Data
{
    public class TourPlannerDbContext : DbContext
    {
        private readonly ILogger<TourPlannerDbContext> _logger;

        private readonly DatabaseConfig _databaseConfig;
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourDetails> TourDetails { get; set; }
        public DbSet<TourAttributes> TourAttributes { get; set; }
        public DbSet<TourLog> TourLogs { get; set; }

        public TourPlannerDbContext(DbContextOptions<TourPlannerDbContext> options, DatabaseConfig databaseConfig, ILogger<TourPlannerDbContext> logger)
            : base(options)
        {
            _databaseConfig = databaseConfig;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            try
            {
                optionsBuilder.UseNpgsql(
                    $"Host={_databaseConfig.Host};Port={_databaseConfig.Port};Database={_databaseConfig.Database};Username={_databaseConfig.Username};Password={_databaseConfig.Password}"
                );
            } 
            catch (NpgsqlException e)
            {
                if(e.InnerException != null)
                {
                    _logger.LogError(e.InnerException, "Inner exception detail: {Message}", e.InnerException.Message);
                }
                _logger.LogError(e, "Unable to establish a connection to the database: {Message}", e.Message);
            }
            catch (Exception e) 
            {
                _logger.LogError(e, "Unable to establish a connection to the database.");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            try
            {
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
            catch (ArgumentException e)
            {
                _logger.LogError(e, "An error occurred while configuring the entity model: {Message}", e.Message);
            }
            catch (InvalidOperationException e)
            {
                _logger.LogError(e, "An error occurred while configuring the entity model: {Message}", e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating database: {Message}", e.Message);
            }
        }
    }
}
