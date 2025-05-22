using System;

namespace TourPlanner.UILayer.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public TourManagementViewModel TourManagementViewModel { get; } 
        public TourLogsManagementViewModel TourLogsManagementViewModel { get; }
        public bool ShowSearchBar { get; set; } = false;
        public EventHandler CreateTourLog;

        public HomeViewModel(
            TourManagementViewModel tourManagementViewModel,
            TourLogsManagementViewModel tourLogsManagementViewModel
        ) {
            TourManagementViewModel = tourManagementViewModel;
            TourLogsManagementViewModel = tourLogsManagementViewModel;
            ShowSearchBar = false;
        }
    }
}
