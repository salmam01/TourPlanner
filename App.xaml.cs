using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.DataLayer.Data;
using TourPlanner.DataLayer.Repositories.TourLogRepository;
using TourPlanner.DataLayer.Repositories.TourRepository;
using TourPlanner.UILayer.Events;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

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

            services.AddSingleton<TourService>();
            services.AddSingleton<TourLogService>();

            services.AddSingleton<MainWindow>(s => new MainWindow()
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
}
