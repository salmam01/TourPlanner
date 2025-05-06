using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourLogsManagementViewModel : BaseViewModel
    {
        private readonly TourLogService _tourLogService;

        public TourLogListViewModel TourLogListViewModel { get; }
        private readonly CreateTourLogViewModel _createTourLogViewModel;

        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;
        private TourLog _selectedTourLog;

        public RelayCommand CreateTourLogCommand => new RelayCommand(
            execute => CreateTourLog()
        );
        public RelayCommand DeleteTourLogCommand => new RelayCommand(
            execute => DeleteTourLog(), 
            canExecute => _selectedTourLog != null
        );
        public RelayCommand EditTourLogCommand => new RelayCommand(
            execute => UpdateTourLog(), 
            canExecute => _selectedTourLog != null
        );

        public TourLogsManagementViewModel(
            CreateTourLogViewModel createTourLogViewModel,
            TourLogListViewModel tourLogListViewModel,
            EventAggregator eventAggregator,
            TourLogService tourLogService
        ) {
            TourLogListViewModel = tourLogListViewModel;
            _createTourLogViewModel = createTourLogViewModel;
            _eventAggregator = eventAggregator;
            _tourLogService = tourLogService;

            _eventAggregator.Subscribe<Tour>(OnTourSelected);
            TourLogListViewModel.TourLogSelected += OnTourLogSelected;
            _createTourLogViewModel.TourLogCreated += OnTourLogCreated;
            _createTourLogViewModel.TourLogUpdated += OnTourLogUpdated;
            _createTourLogViewModel.Cancelled += OnCancel;
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            TourLogListViewModel.ReloadTourLogs(_tourLogService.GetTourLogs(tour));
        }

        public void OnTourLogSelected(object sender, TourLog tourLog)
        {
            if (tourLog == null) return;
            _selectedTourLog = tourLog;
        }

        public void OnTourLogCreated(object sender, TourLog tourLog)
        {
            if (tourLog == null) return;
            _tourLogService.CreateTourLog(_selectedTour.Id, tourLog);
            TourLogListViewModel.ReloadTourLogs(_tourLogService.GetTourLogs(_selectedTour));
            _eventAggregator.Publish("ShowHome");
        }

        public void OnTourLogUpdated(object sender, TourLog tourLog)
        {
            if(tourLog == null) return;
            TourLogListViewModel.OnTourLogUpdated(tourLog);
            _eventAggregator.Publish("ShowHome");
        }

        public void OnCancel(object sender, EventArgs e)
        {
            _eventAggregator.Publish("ShowHome");
        }

        private void CreateTourLog() 
        {
            if (_selectedTour == null) return;
            _eventAggregator.Publish("ShowCreateTourLog");
        }

        private void UpdateTourLog()
        {
            if (_selectedTourLog == null || _selectedTour == null) return;
            _createTourLogViewModel.LoadTourLog(_selectedTourLog);
            _eventAggregator.Publish("ShowCreateTourLog");
        }

        private void DeleteTourLog() 
        {
            if (_selectedTourLog == null || _selectedTour == null) return;

            MessageBoxResult result = MessageBox.Show(
                "Are you sure you would like to delete this tour log?",
                $"Delete Tour Log {_selectedTourLog.Id}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (result != MessageBoxResult.Yes) return;

            _tourLogService.DeleteTourLog(_selectedTourLog);
            TourLogListViewModel.ReloadTourLogs(_tourLogService.GetTourLogs(_selectedTour));
            _selectedTourLog = null;
        }
    }
}
