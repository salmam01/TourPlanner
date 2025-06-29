using System;

namespace TourPlanner.UI.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public TourManagementViewModel TourManagementViewModel { get; } 
        public TourNavbarViewModel TourNavbarViewModel { get; }

        public HomeViewModel(
            TourManagementViewModel tourManagementViewModel,
            TourNavbarViewModel tourNavbarViewModel
        ) {
            TourManagementViewModel = tourManagementViewModel;
            TourNavbarViewModel = tourNavbarViewModel;
        }
    }
}
