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
using TourPlanner.UI.ViewModels;

namespace TourPlanner.UI.Views
{
    /// <summary>
    /// Interaction logic for Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        public Map()
        {
            InitializeComponent();
            IntializeAsync();
        }

        private async void IntializeAsync()
        {
            await webView.EnsureCoreWebView2Async(null);

            if (this.DataContext is MapViewModel viewModel)
            {
                string baseDir = viewModel.BaseDirectory;
                string filePath = System.IO.Path.Combine(baseDir, "TourPlanner.UI", "Leaflet", "map.html");
                Debug.WriteLine($"File path: {filePath}");

                webView.CoreWebView2.Navigate(filePath);
            }
        }
    }
}
