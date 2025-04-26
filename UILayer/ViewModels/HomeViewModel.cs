using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Stores;
using TourPlanner.UILayer.Views;

namespace TourPlanner.UILayer.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        public TourManagementViewModel TourManagementViewModel { get; }
        public TourListViewModel TourListViewModel { get; }
        public TourLogsManagementViewModel TourLogsManagementViewModel { get; }
        public SearchBarViewModel SearchBarViewModel { get; }

        public HomeViewModel(
            TourManagementViewModel tourManagementViewModel,
            TourListViewModel tourListViewModel,
            TourLogsManagementViewModel tourLogsManagementViewModel,
            SearchBarViewModel searchBarViewModel
            )
        {
            TourManagementViewModel = tourManagementViewModel;
            TourListViewModel = tourListViewModel;
            TourLogsManagementViewModel = tourLogsManagementViewModel;
            SearchBarViewModel = searchBarViewModel;


        }

    }
}
