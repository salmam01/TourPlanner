using System;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Views;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private TourService _tourService;
        private CreateTourViewModel _createTourViewModel;
        private TourListViewModel _tourListViewModel;

        private Tour _selectedTour;
        private bool _showCreateTour;

        public ICommand DeleteTourCommand => new RelayCommand(execute => OnDeleteTour());

        public Tour SelectedTour
        {
            get => _selectedTour;
            set 
            {
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }

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

        public TourManagementViewModel()
        {
            _tourService = new TourService();
            _createTourViewModel = new CreateTourViewModel();
            _tourListViewModel = new TourListViewModel();

            _createTourViewModel.TourCreated += OnTourCreated;
        }

        public void OnCreateTour()
        {
            ShowCreateTour = true;
        }

        public void OnTourCreated(object sender, Tour tour)
        {
            if (tour == null) return;
            _tourService.CreateTour(tour);
            ShowCreateTour = false;
        }

        public void OnDeleteTour()
        {
            _tourService.DeleteTour(_selectedTour);
        }
    }
}
