using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using TourPlanner.Models.Configuration;
using Microsoft.EntityFrameworkCore;

namespace TourPlanner.DAL.Data
{
    public class TourPlannerDbContextFactory : IDesignTimeDbContextFactory<TourPlannerDbContext>
    {
        public TourPlannerDbContext CreateDbContext(string[] args)
        {
            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, false)
                .Build();

            DatabaseConfig databaseConfig = config.GetSection("Database").Get<DatabaseConfig>();
            
            if (databaseConfig == null)
            {
                throw new InvalidOperationException("Connection string is missing or empty in [appsettings.json].");
            }

            string connectionString = databaseConfig.ConnectionString;
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string is missing or empty in [appsettings.json].");
            }

            DbContextOptions<TourPlannerDbContext> options = new DbContextOptionsBuilder<TourPlannerDbContext>()
                    .UseNpgsql(connectionString)
                    .Options;

            return new TourPlannerDbContext(options);
        }
    }
}