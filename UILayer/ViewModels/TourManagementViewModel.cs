using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ICommand DeleteTourCommand => new RelayCommand(execute => OnDeleteTour());

        public TourManagementViewModel(CreateTourViewModel createTourViewModel, TourListViewModel tourListViewModel)
        {
            _tourService = new TourService();
            CreateTourViewModel = createTourViewModel;
            TourListViewModel = tourListViewModel;
            Console.WriteLine("TourManagementViewModel initialized");

            CreateTourViewModel.TourCreated += OnTourCreated;
        }

        public void OnTourCreated(object sender, Tour tour)
        {
            _tourService.CreateTour(tour);
            TourListViewModel.OnTourCreated(tour);

            MessageBox.Show($"Tour created! \n" +
                $"Name: {tour.Name}\n" +
                $"Date: {tour.Date}\n" +
                $"Description: {tour.Description}\n" +
                $"Transport Type: {tour.TransportType}\n" +
                $"From: {tour.From}\n" +
                $"To: {tour.To}");
        }

        public void OnDeleteTour()
        {
            MessageBox.Show($"Tour {_selectedTour.Name} deleted!");

            _tourService.DeleteTour(_selectedTour);
            TourListViewModel.OnTourDeleted(_selectedTour);
        }
    }
}
