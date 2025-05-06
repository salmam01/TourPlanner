using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;

        private CreateTourViewModel _createTourViewModel;
        public TourListViewModel TourListViewModel { get; }
        public SearchBarViewModel SearchBarViewModel { get; }

        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;

        public ICommand CreateTourCommand => new RelayCommand(
            execute => CreateTour()
        );

        public ICommand EditTourCommand => new RelayCommand(
            execute => EditTour()
        );
        
        public ICommand DeleteTourCommand => new RelayCommand(
            execute => DeleteTour()
        );


        public TourManagementViewModel(
            CreateTourViewModel createTourViewModel,
            TourListViewModel tourListViewModel,
            SearchBarViewModel searchBarViewModel,
            EventAggregator eventAggregator,
            TourService tourService
        ) {
            _createTourViewModel = createTourViewModel;
            TourListViewModel = tourListViewModel;
            SearchBarViewModel = searchBarViewModel;
            _eventAggregator = eventAggregator;
            _tourService = tourService;

            _createTourViewModel.TourCreated += OnTourCreated;
            _createTourViewModel.TourUpdated += OnTourUpdated;
            _eventAggregator.Subscribe<Tour>(OnTourSelected);
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
        }

        public void OnTourCreated(object sender, Tour tour)
        {
            if (tour == null) return;
            TourListViewModel.OnTourCreated(tour);
            MessageBox.Show($"Tour {tour.Name} created!");
            _eventAggregator.Publish("ShowHome");
            _tourService.CreateTour(tour);
        }

        public void OnTourUpdated(object sender, Tour tour)
        {
            if (tour == null) return;
            TourListViewModel.OnTourUpdated(tour);
            _eventAggregator.Publish("ShowHome");

            //  TODO: Implement this
            //_tourService.UpdateTour(tour, tour.Name, tour.Date, tour.Description, tour.TransportType, tour.From, tour.To);
        }

        public void CreateTour()
        {
            _eventAggregator.Publish("ShowCreateTour");
        }

        public void EditTour()
        {
            if(_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to edit.");
                return;
            }
            _createTourViewModel.LoadTour(_selectedTour);
            _eventAggregator.Publish("ShowCreateTour");
        }

        public void DeleteTour()
        {
            if(_selectedTour == null)
            {
                MessageBox.Show("Please select a tour to delete.");
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you would like to delete this tour log?",
                $"Delete Tour {_selectedTour.Name}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result != MessageBoxResult.Yes) return;
            TourListViewModel.OnTourDeleted(_selectedTour);
            _tourService.DeleteTour(_selectedTour);
        }
    }
}
