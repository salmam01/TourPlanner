using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TourPlanner.BL.Services;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourLogsManagementViewModel : BaseViewModel
    {
        private readonly TourLogService _tourLogService;
        public TourLogListViewModel TourLogListViewModel { get; }
        private readonly CreateTourLogViewModel _createTourLogViewModel;
        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;
        private TourLog _selectedTourLog;
        
        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set {
                if (_selectedTourLog == value) return;
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
            }
        }

        private readonly RelayCommand _createTourLogCommand;
        private readonly RelayCommand _deleteTourLogCommand;
        private readonly RelayCommand _editTourLogCommand;
        
        public RelayCommand CreateTourLogCommand => _createTourLogCommand;
        public RelayCommand DeleteTourLogCommand => _deleteTourLogCommand;
        public RelayCommand EditTourLogCommand => _editTourLogCommand;

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
        
            _createTourLogCommand = new RelayCommand(
                execute => CreateTourLog()
            );
            _deleteTourLogCommand = new RelayCommand(
                execute => DeleteTourLog(),
                canExecute => _selectedTourLog != null
            );
            _editTourLogCommand = new RelayCommand(
                execute => UpdateTourLog(execute as TourPlanner.Models.Entities.TourLog),
                canExecute => _selectedTour != null
            );
        
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
            SelectedTourLog = tourLog;
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

            _tourLogService.UpdateTourLog(tourLog);
            TourLogListViewModel.ReloadTourLogs(_tourLogService.GetTourLogs(_selectedTour).ToList());
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

        private void UpdateTourLog(TourLog tourLogToEdit)
        {
            if (tourLogToEdit == null || _selectedTour == null) return;
            _createTourLogViewModel.LoadTourLog(tourLogToEdit);
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
            Log.Information("Tour Log deleted => {@_selectedTourLog}", _selectedTourLog);
            _selectedTourLog = null;
        }
    }
}
