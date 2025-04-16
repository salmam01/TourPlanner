using System;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Commands;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;

        public CreateTourViewModel CreateTourViewModel;
        public TourListViewModel TourListViewModel;

        private Tour _selectedTour;

        public ICommand DeleteTourCommand => new RelayCommand(execute => OnDeleteTour());

        public Tour SelectedTour
        {
            get => _selectedTour;
            set 
            {
                if (_selectedTour == value) return;
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }

        public TourManagementViewModel()
        {
            _tourService = new TourService();
            CreateTourViewModel = new CreateTourViewModel();
            TourListViewModel = new TourListViewModel();

            CreateTourViewModel.TourCreated += OnTourCreated;
        }

        public void OnTourCreated(object sender, Tour tour)
        {
            _tourService.CreateTour(tour);
            TourListViewModel.OnTourCreated(tour);
            
            MessageBox.Show($"Tour created!");
        }

        public void OnDeleteTour()
        {
            MessageBox.Show($"Tour {_selectedTour.Name} deleted!");

            _tourService.DeleteTour(_selectedTour);
            TourListViewModel.OnTourDeleted(_selectedTour);
        }
    }
}
