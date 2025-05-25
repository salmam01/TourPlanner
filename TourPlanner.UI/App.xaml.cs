using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TourPlanner.BL.Services;
using TourPlanner.Models.Configuration;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Repositories.TourAttributesRepository;
using TourPlanner.DAL.Repositories.TourDetailsRepository;
using TourPlanner.DAL.Repositories.TourLogRepository;
using TourPlanner.DAL.Repositories.TourRepository;
using TourPlanner.UI.Events;
using TourPlanner.UI.ViewModels;
using TourPlanner.UI.Views;
using Serilog;

namespace TourPlanner.UI;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

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

        //  Logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(config)
            .CreateLogger();

        //  Dependency Injection
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton(databaseConfig);

        services.AddSingleton<MainWindowViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<EventAggregator>();

        services.AddSingleton<TourManagementViewModel>();
        services.AddSingleton<CreateTourViewModel>();
        services.AddSingleton<TourListViewModel>();
        services.AddSingleton<SearchBarViewModel>();

        services.AddSingleton<TourLogsManagementViewModel>();
        services.AddSingleton<CreateTourLogViewModel>();
        services.AddSingleton<TourLogListViewModel>();

        services.AddDbContext<TourPlannerDbContext>();
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ITourLogRepository, TourLogRepository>();
        services.AddScoped<ITourDetailsRepository, TourDetailsRepository>();
        services.AddScoped<ITourAttributesRepository, TourAttributesRepository>();

        services.AddSingleton<TourService>();
        services.AddSingleton<TourLogService>();
        services.AddSingleton<TourDetailsService>();

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