using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models.Entities;
using TourPlanner.BL.Services;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourLogListViewModel : BaseViewModel
    {
        private string _searchQuery;
        public string SearchQuery
        {
            get => _searchQuery;
            set {
                _searchQuery = value;
                OnPropertyChanged(nameof(SearchQuery));
            }
        }

        private readonly TourLogService _tourLogService;
        public event EventHandler<TourLog> TourLogSelected;
        private Guid _currentTourId;
        public Guid CurrentTourId
        {
            get => _currentTourId;
            set {
                if (_currentTourId == value) return;
                _currentTourId = value;
                OnPropertyChanged(nameof(CurrentTourId));
                LoadTourLogsForTour(_currentTourId);
            }
        }

        private void LoadTourLogsForTour(Guid tourId)
        {
            Tour tour = new Tour { Id = tourId };
            List<TourLog> logs = _tourLogService.GetTourLogs(tour).ToList();
            ReloadTourLogs(logs);
        }

        private ObservableCollection<TourLog> _tourLogs;
        private readonly EventAggregator _eventAggregator;
        public ObservableCollection<TourLog> TourLogs
        {
            get => _tourLogs;
            set {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }
        
        public TourLogListViewModel(TourLogService tourLogService, EventAggregator eventAggregator)
        {
            _tourLogService = tourLogService;
            _eventAggregator = eventAggregator;
            _tourLogs = new ObservableCollection<TourLog>();

            _eventAggregator.Subscribe<string>(query =>
            {
                Console.WriteLine($"Searching for: {query}");
                SearchQuery = query;
                var filteredLogs = _tourLogService.SearchTourLogs(SearchQuery).ToList();
                ReloadTourLogs(filteredLogs);
            });
        }

        public void ReloadTourLogs(IEnumerable<TourLog> tourLogs)
        {
           // Console.WriteLine($"Anzahl der gefilterten Logs: {tourLogs.Count()}");
            _tourLogs.Clear();
            foreach (TourLog tourLog in tourLogs) {
                _tourLogs.Add(tourLog);
            }
            OnPropertyChanged(nameof(TourLogs));
           // Console.WriteLine($"Neue Anzahl in ObservableCollection: {_tourLogs.Count}");
        }
        
         private TourLog _selectedTourLog;
        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set {
                if (_selectedTourLog == value) return;
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
                TourLogSelected?.Invoke(this, _selectedTourLog);
            }
        }

        private RelayCommand _editTourLogCommand;
        public RelayCommand EditTourLogCommand => _editTourLogCommand ??= new RelayCommand(
            execute => EditTourLog(),
            canExecute => SelectedTourLog != null
        );

        private RelayCommand _deleteTourLogCommand;
        public RelayCommand DeleteTourLogCommand => _deleteTourLogCommand ??= new RelayCommand(
            execute => DeleteTourLog(),
            canExecute => SelectedTourLog != null
        );

        private void EditTourLog()
        {
            if (SelectedTourLog == null) return;
            _tourLogService.UpdateTourLog(SelectedTourLog);
            LoadTourLogsForTour(_currentTourId);
        }

        private void DeleteTourLog()
        {
            if (SelectedTourLog == null) return;
            _tourLogService.DeleteTourLog(SelectedTourLog);
            _tourLogs.Remove(SelectedTourLog);
            OnPropertyChanged(nameof(TourLogs));
        }
    }
}