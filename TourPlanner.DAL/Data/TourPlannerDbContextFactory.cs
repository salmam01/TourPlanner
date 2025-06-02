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

            //  one connection string
            DatabaseConfig databaseConfig = config.GetSection("Database").Get<DatabaseConfig>();

            string connectionString = 
                $"Host={databaseConfig.Host};" +
                $"Port={databaseConfig.Port};" +
                $"Database={databaseConfig.Database};" +
                $"Username={databaseConfig.Username};" +
                $"Password={databaseConfig.Password}";

            var options = new DbContextOptionsBuilder<TourPlannerDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new TourPlannerDbContext(options);
        }
    }
}
