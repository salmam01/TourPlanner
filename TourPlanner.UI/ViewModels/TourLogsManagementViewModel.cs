using Serilog;
using System.Windows;
using System.Windows.Input;
using TourPlanner.UI.Services;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;
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

        public ICommand CreateTourLogCommand => new RelayCommand(
            execute => CreateTourLog()
        );
        public RelayCommand DeleteTourLogCommand => new RelayCommand(
                execute => DeleteTourLog(),
                canExecute => _selectedTourLog != null
        );
        public RelayCommand EditTourLogCommand => new RelayCommand(
                execute => UpdateTourLog(execute as TourPlanner.Models.Entities.TourLog),
                canExecute => _selectedTour != null
        );

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

            _eventAggregator.Subscribe<TourEvent>(e =>
            {
                EventHandler(e);
            });

            TourLogListViewModel.TourLogSelected += OnTourLogSelected;
            _createTourLogViewModel.TourLogCreated += OnTourLogCreated;
            _createTourLogViewModel.TourLogEdited += OnTourLogEdited;
            _createTourLogViewModel.Cancelled += OnCancel;
        }

        private void EventHandler(TourEvent tourEvent)
        {
            switch (tourEvent.Type)
            {
                case TourEvent.EventType.SelectTour:
                    OnTourSelected(tourEvent.Tour);
                    break;
                case TourEvent.EventType.Reload:
                    OnReload();
                    break;
                case TourEvent.EventType.TourDeleted:
                    OnReload();
                    break;
                case TourEvent.EventType.AllToursDeleted:
                    OnReload();
                    break;
            }
        }

        private void OnReload()
        {
            TourLogListViewModel.Clear();
            _selectedTour = null;
            _selectedTourLog = null;
        }

        public void ReloadLogList(Tour tour)
        {
            Result result = _tourLogService.GetAllTourLogs(tour);
            if (result.Code != Result.ResultCode.Success)
            {
                return;
            }
            if (result.Data is not List<TourLog> logs)
            {
                return;
            }

            TourLogListViewModel.ReloadTourLogs(logs);
            _selectedTourLog = null;
        }

        public void OnTourSelected(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;
            ReloadLogList(tour);
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
                _eventAggregator.Publish(new TourEvent(TourEvent.EventType.LogCreated, _selectedTour));
                ReloadLogList(_selectedTour);
            }
            else
                ShowErrorMessage(result);

            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        public void OnTourLogEdited(object sender, TourLog tourLog)
        {
            if(tourLog == null) return;

            Result result = _tourLogService.UpdateTourLog(tourLog);

            if (result.Code == Result.ResultCode.Success)
            {
                _tourService.RecalculateTourAttributes(_selectedTour);
                ReloadLogList(_selectedTour);
            }
            else
                ShowErrorMessage(result);

            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        public void OnCancel(object sender, EventArgs e)
        {
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.Home));
        }

        private void CreateTourLog() 
        {
            if (_selectedTour == null) return;
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.CreateTourLog));
        }

        private void UpdateTourLog(TourLog tourLogToEdit)
        {
            if (tourLogToEdit == null || _selectedTour == null) return;
            _createTourLogViewModel.LoadTourLog(tourLogToEdit);
            _eventAggregator.Publish(new NavigationEvent(NavigationEvent.Destination.CreateTourLog));
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
                ReloadLogList(_selectedTour);
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
