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
using TourPlanner.BL.API;
using TourPlanner.BL.Utils.Helpers;
using QuestPDF;

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
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        //  Load configuration from appsettings.json
        IConfiguration config = new ConfigurationBuilder()
            // Set the file to Content and Copy-Always
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        //  Bind configuration to the model classes
        DatabaseConfig databaseConfig = config.GetSection("Database").Get<DatabaseConfig>();
        PathsConfig pathsConfig = config.GetSection("Paths").Get<PathsConfig>();
        ApiConfig apiConfig = config.GetSection("Api").Get<ApiConfig>();

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

        //  Base classes
        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<EventAggregator>();
        services.AddSingleton<LogViewerViewModel>();

        //  Tour
        services.AddSingleton<TourManagementViewModel>();
        services.AddSingleton<CreateTourViewModel>();
        services.AddSingleton<TourListViewModel>();
        services.AddSingleton<TourDetailsViewModel>();
        services.AddTransient<SearchBarViewModel>();

        //  Navigation bar
        services.AddSingleton<TourNavbarViewModel>();
        services.AddSingleton(mvm =>
            new MapViewModel(
                mvm.GetRequiredService<EventAggregator>(),
                mvm.GetRequiredService<OpenRouteService>(),
                mvm.GetRequiredService<LeafletHelper>(),
                pathsConfig.BaseDirectory
            )
        );

        //  Tour logs
        services.AddSingleton<TourLogsManagementViewModel>();
        services.AddSingleton<CreateTourLogViewModel>();
        services.AddSingleton<TourLogListViewModel>();

        //  Add DbContext
        services.AddDbContext<TourPlannerDbContext>(options =>
        {
            options.UseNpgsql(databaseConfig.ConnectionString);
        });

        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ITourLogRepository, TourLogRepository>();
        services.AddScoped<ITourAttributesRepository, TourAttributesRepository>();

        //  Internal Services
        services.AddSingleton<TourService>();
        services.AddSingleton<TourLogService>();
        services.AddSingleton<TourAttributesService>();
        services.AddSingleton<ImportExportService>();
        services.AddSingleton<ReportGenerationService>();

        //  API Services
        services.AddSingleton<Parser>();
        services.AddSingleton(s =>
            new OpenRouteService(
                apiConfig.OpenRouteServiceKey,
                s.GetRequiredService<Parser>(),
                apiConfig.BaseUrl,
                apiConfig.FocusPointLat,
                apiConfig.FocusPointLon,
                s.GetRequiredService<ILogger<OpenRouteService>>()
            )
        );

        //  Leaflet
        services.AddSingleton<LeafletHelper>();

        services.AddSingleton(s => new MainWindow
        {
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });
        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        //  Create the Database based on Migrations
        using (IServiceScope scope = _serviceProvider.CreateScope())
        {
            scope.ServiceProvider.GetRequiredService<TourPlannerDbContext>().Database.Migrate();
        }

        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
        base.OnStartup(e);
    }
}