using Serilog;
using System.Windows;
using TourPlanner.BL.Services;
using TourPlanner.BL.Utils;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourLogsManagementViewModel : BaseViewModel
    {
        private readonly TourService _tourService;
        private readonly TourLogService _tourLogService;
        public TourLogListViewModel TourLogListViewModel { get; }
        public SearchBarViewModel SearchBarViewModel { get; }
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
            SearchBarViewModel searchBarViewModel,
            EventAggregator eventAggregator,
            TourService tourService,
            TourLogService tourLogService
        ) {
            TourLogListViewModel = tourLogListViewModel;
            _createTourLogViewModel = createTourLogViewModel;
            _eventAggregator = eventAggregator;
            _tourService = tourService;
            _tourLogService = tourLogService;
            SearchBarViewModel = searchBarViewModel;

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
            TourLogListViewModel.ReloadTourLogs(tour.TourLogs);
        }

        public void OnTourLogSelected(object sender, TourLog tourLog)
        {
            SelectedTourLog = tourLog;
        }

        public void OnTourLogCreated(object sender, TourLog tourLog)
        {
            if (tourLog == null) return;

            Result result = _tourLogService.CreateTourLog(_selectedTour.Id, tourLog);

            if (result.Code == Result.ResultCode.Success)
            {
                _tourService.RecalculateTourAttributes(_selectedTour);
                TourLogListViewModel.ReloadTourLogs(_selectedTour.TourLogs);
            }
            else
                ShowErrorMessage(result);

            _eventAggregator.Publish("ShowHome");
        }

        public void OnTourLogUpdated(object sender, TourLog tourLog)
        {
            if(tourLog == null) return;

            Result result = _tourLogService.UpdateTourLog(tourLog);

            if (result.Code == Result.ResultCode.Success)
            {
                _tourService.RecalculateTourAttributes(_selectedTour);
                TourLogListViewModel.ReloadTourLogs(_selectedTour.TourLogs);
            }
            else
                ShowErrorMessage(result);

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

            MessageBoxResult warning = MessageBox.Show(
                "Are you sure you would like to delete this tour log?",
                $"Delete Tour Log {_selectedTourLog.Id}",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );
            if (warning != MessageBoxResult.Yes) return;

            Result deleteResult = _tourLogService.DeleteTourLog(_selectedTourLog);
            if (deleteResult.Code == Result.ResultCode.Success)
            {
                _tourService.RecalculateTourAttributes(_selectedTour);
                TourLogListViewModel.ReloadTourLogs(_selectedTour.TourLogs);
                _selectedTourLog = null;
            }
            else
                ShowErrorMessage(deleteResult);
        }

        private void ShowErrorMessage(Result result)
        {
            string message = string.Empty;

            switch (result.Code)
            {
                case Result.ResultCode.NullError:
                    message = "Please provide valid Tour Log details.";
                    break;
                case Result.ResultCode.DatabaseError:
                    message = "Database error occurred. Please try again later.";
                    break;
                case Result.ResultCode.UnknownError:
                    message = "An unknown error occurred.";
                    break;
            }

            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
