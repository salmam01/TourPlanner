using System;

namespace TourPlanner.UI.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public TourManagementViewModel TourManagementViewModel { get; } 
        public TourLogsManagementViewModel TourLogsManagementViewModel { get; }
        public TourNavbarViewModel TourNavbarViewModel { get; }

        public bool ShowSearchBar { get; set; } = false;

        public HomeViewModel(
            TourManagementViewModel tourManagementViewModel,
            TourLogsManagementViewModel tourLogsManagementViewModel,
            TourNavbarViewModel tourNavbarViewModel
        ) {
            TourManagementViewModel = tourManagementViewModel;
            TourLogsManagementViewModel = tourLogsManagementViewModel;
            TourNavbarViewModel = tourNavbarViewModel;
            ShowSearchBar = false;
        }
    }
}
