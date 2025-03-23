using System;
using System.Windows.Controls;
using System.Diagnostics;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner.UILayer.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private readonly TourListViewModel _tourListViewModel;
        private readonly TourLogsManagementViewModel _tourLogsViewModel;

        public Home()
        {
            InitializeComponent();
            
            // Initialize ViewModels
            _tourListViewModel = new TourListViewModel();
            _tourLogsViewModel = new TourLogsManagementViewModel();

            // Connect ViewModels
            _tourListViewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(TourListViewModel.SelectedTour))
                {
                    Debug.WriteLine($"Home: TourList SelectedTour changed to: {_tourListViewModel.SelectedTour?.Name ?? "null"}");
                    _tourLogsViewModel.SelectedTour = _tourListViewModel.SelectedTour;
                }
            };

            // Set DataContext for views
            if (TourListView != null)
            {
                Debug.WriteLine("Setting TourListView DataContext");
                TourListView.DataContext = _tourListViewModel;
            }
            
            if (TourLogsView != null)
            {
                Debug.WriteLine("Setting TourLogsView DataContext");
                TourLogsView.DataContext = _tourLogsViewModel;
            }
            else
            {
                Debug.WriteLine("WARNING: TourLogsView is null!");
            }
        }
    }
}
