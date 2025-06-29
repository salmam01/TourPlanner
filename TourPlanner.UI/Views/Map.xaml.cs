using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TourPlanner.UI.Events;
using TourPlanner.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace TourPlanner.UI.Views
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        private EventAggregator _eventAggregator;
        public Map()
        {
            InitializeComponent();

            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                IntializeAsync();

                if (_eventAggregator == null)
                {
                    _eventAggregator = ((App)Application.Current).ServiceProvider.GetService<EventAggregator>();
                    _eventAggregator.Subscribe<MapUpdatedEvent>(OnMapUpdated);
                }
            }
        }

        private async void IntializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);

            if (this.DataContext is MapViewModel viewModel)
            {
                string baseDir = viewModel.BaseDirectory;
                string filePath = System.IO.Path.Combine(baseDir, "TourPlanner.UI", "Leaflet", "map.html");
                webView.CoreWebView2.Navigate(filePath);
            }
        }

        private void OnMapUpdated(MapUpdatedEvent mapUpdatedEvent)
        {
            webView.CoreWebView2.Reload();
        }
    }
}
