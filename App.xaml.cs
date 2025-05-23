using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.Configuration;
using TourPlanner.DataLayer.Data;
using TourPlanner.DataLayer.Repositories.TourAttributesRepository;
using TourPlanner.DataLayer.Repositories.TourDetailsRepository;
using TourPlanner.DataLayer.Repositories.TourLogRepository;
using TourPlanner.DataLayer.Repositories.TourRepository;
using TourPlanner.UILayer.Events;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner;

/// <summary>
///     Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private readonly IServiceProvider _serviceProvider;

        public App()
        {
            // Load configuration from appsettings.json
            IConfiguration config = new ConfigurationBuilder()
                // Set the file to Content and Copy-Always
                .AddJsonFile("appsettings.json", false, true)
                .Build();

            // Bind configuration to the model classes
            DatabaseConfig databaseConfig = config.GetSection("Database").Get<DatabaseConfig>();

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
        services.AddSingleton(s =>
            new TourListViewModel(
                s.GetRequiredService<EventAggregator>(),
                s.GetRequiredService<TourService>())
        );

        services.AddSingleton(s =>
            new SearchBarViewModel(
                s.GetRequiredService<EventAggregator>())
        );
        
        services.AddSingleton<TourService>();
        services.AddSingleton<TourLogService>();
        services.AddSingleton<TourDetailsService>();
        // Removed: services.AddSingleton<TourAttributesService>(); -- static class, should not be added to DI

        services.AddSingleton(s => new MainWindow {
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });
        _serviceProvider = services.BuildServiceProvider();
    }

    protected override void OnStartup(StartupEventArgs e) {
        MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        MainWindow.Show();
        base.OnStartup(e);
    }
}