using System;

namespace TourPlanner.UILayer.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public TourManagementViewModel TourManagementViewModel { get; }
        public TourLogsManagementViewModel TourLogsManagementViewModel { get; }

        public EventHandler CreateTourLog;

        public HomeViewModel(
            TourManagementViewModel tourManagementViewModel,
            TourLogsManagementViewModel tourLogsManagementViewModel
        ) {
            TourManagementViewModel = tourManagementViewModel;
            TourLogsManagementViewModel = tourLogsManagementViewModel;
        }
    }
}
