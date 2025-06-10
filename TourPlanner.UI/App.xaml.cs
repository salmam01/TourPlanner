using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
using TourPlanner.Models.Configuration;
using TourPlanner.BL.Services;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Repositories.TourAttributesRepository;
using TourPlanner.DAL.Repositories.TourLogRepository;
using TourPlanner.DAL.Repositories.TourRepository;
using TourPlanner.UI.Events;
using TourPlanner.UI.ViewModels;
using TourPlanner.UI.Views;

namespace TourPlanner.UI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;
    public IServiceProvider ServiceProvider => _serviceProvider;

    public App()
    {
        //  Load configuration from appsettings.json
        IConfiguration config = new ConfigurationBuilder()
            // Set the file to Content and Copy-Always
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        //  Bind configuration to the model classes
        DatabaseConfig databaseConfig = config.GetSection("Database").Get<DatabaseConfig>();
        PathsConfig basePath = config.GetSection("Paths").Get<PathsConfig>();

        //  Configure Serilog globally
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        //  Dependency Injection
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton(databaseConfig);
        services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            loggingBuilder.AddSerilog();
        });

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<EventAggregator>();
        services.AddSingleton<LogViewerViewModel>();

        services.AddSingleton<TourManagementViewModel>();
        services.AddSingleton<CreateTourViewModel>();
        services.AddSingleton<TourListViewModel>();
        services.AddTransient<SearchBarViewModel>();

        services.AddSingleton<TourLogsManagementViewModel>();
        services.AddSingleton<CreateTourLogViewModel>();
        services.AddSingleton<TourLogListViewModel>();

        //  Change
        services.AddDbContext<TourPlannerDbContext>( options =>
        {
            options.UseNpgsql(
                $"Host={databaseConfig.Host};" +
                $"Port={databaseConfig.Port};" +
                $"Database={databaseConfig.Database};" +
                $"Username={databaseConfig.Username};" +
                $"Password={databaseConfig.Password}"
            );
        });
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ITourLogRepository, TourLogRepository>();
        services.AddScoped<ITourAttributesRepository, TourAttributesRepository>();

        services.AddSingleton<TourService>();
        services.AddSingleton<TourLogService>();
        services.AddSingleton<TourAttributesService>();
        services.AddSingleton<TourImportExportService>();

        services.AddSingleton(s => new MainWindow
        {
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });
        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
        base.OnStartup(e);
    }
}