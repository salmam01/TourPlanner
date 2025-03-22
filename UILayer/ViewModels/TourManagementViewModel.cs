using System;
using TourPlanner.UILayer.Commands;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        public RelayCommand CreateTourCommand => new RelayCommand(execute => OnCreateTour());
        private bool _showCreateTour;
        public bool ShowCreateTour
        {
            get
            {
                return _showCreateTour;
            }
            set
            {
                _showCreateTour = value;
                OnPropertyChanged();
            }
        }


        public void OnCreateTour()
        {
            ShowCreateTour = true;
        }
    }
}
