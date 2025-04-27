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
        public TourListViewModel TourListViewModel { get; }
        public SearchBarViewModel SearchBarViewModel { get; }
        private Tour _selectedTour;

        public ICommand DeleteTourCommand => new RelayCommand(execute => DeleteTour());


        public TourManagementViewModel(CreateTourViewModel createTourViewModel, TourListViewModel tourListViewModel, SearchBarViewModel searchBarViewModel)
        {
            _tourService = new TourService();
            CreateTourViewModel = createTourViewModel;
            TourListViewModel = tourListViewModel;
            SearchBarViewModel = searchBarViewModel;

            CreateTourViewModel.TourCreated += OnTourCreated;
            TourListViewModel.TourSelected += OnTourSelected;
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

        public void OnTourSelected(object sender, Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
        }

        public void DeleteTour()
        {
            if(_selectedTour == null)
            {
                return;
            }
            Console.WriteLine($"Tour {_selectedTour.Name} deleted!");

            _tourService.DeleteTour(_selectedTour);
            TourListViewModel.OnTourDeleted(_selectedTour);
        }
    }
}
