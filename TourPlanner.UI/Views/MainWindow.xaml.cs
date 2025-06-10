using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlanner.UI.Views;
using TourPlanner.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TourPlanner.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LogViewerViewModel _logViewerViewModel;

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _logViewerViewModel = ((App)Application.Current).ServiceProvider.GetRequiredService<LogViewerViewModel>();
                LogViewerOverlay.DataContext = _logViewerViewModel;
                Log.Information("LogViewer initialized successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error initializing LogViewer");
                MessageBox.Show($"Error initializing LogViewer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.LeftButton==MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void buttonSize_Click(object sender, RoutedEventArgs e)
        {
            if(WindowState != WindowState.Maximized)
            {
                WindowState = WindowState.Maximized;
            } else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void buttonShowLogs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Log.Information("Show Logs button clicked");
                LogViewerOverlay.Visibility = Visibility.Visible;
                var fadeIn = FindResource("FadeIn") as System.Windows.Media.Animation.Storyboard;
                fadeIn?.Begin(LogViewerOverlay);
                _logViewerViewModel.RefreshLogs();
                Log.Information("Log viewer shown successfully");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error showing log viewer");
                MessageBox.Show($"Error showing log viewer: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void buttonCloseLogs_Click(object sender, RoutedEventArgs e)
        {
            var fadeOut = FindResource("FadeOut") as System.Windows.Media.Animation.Storyboard;
            if (fadeOut != null)
            {
                fadeOut.Completed += (s, args) => LogViewerOverlay.Visibility = Visibility.Collapsed;
                fadeOut.Begin(LogViewerOverlay);
            }
            else
            {
                LogViewerOverlay.Visibility = Visibility.Collapsed;
            }
        }
    }
}
