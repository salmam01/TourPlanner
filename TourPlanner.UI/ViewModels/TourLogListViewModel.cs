using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Services;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourLogListViewModel : BaseViewModel
    {
        public event EventHandler<TourLog> TourLogSelected;

        private Guid _currentTourId;
        public Guid CurrentTourId
        {
            get => _currentTourId;
            set {
                if (_currentTourId == value) return;
                _currentTourId = value;
                OnPropertyChanged(nameof(CurrentTourId));
            }
        }

        private bool _hasNoResults;
        public bool HasNoResults
        {
            get => _hasNoResults;
            set
            {
                if (_hasNoResults == value) return;
                _hasNoResults = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<TourLog> _tourLogs;
        public ObservableCollection<TourLog> TourLogs
        {
            get => _tourLogs;
            set
            {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        private TourLog _selectedTourLog;
        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                if (_selectedTourLog == value) return;
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
                TourLogSelected?.Invoke(this, _selectedTourLog);
            }
        }

        public TourLogListViewModel() {
            _tourLogs = new ObservableCollection<TourLog>();
        }

        public void ReloadTourLogs(IEnumerable<TourLog> tourLogs)
        {
            Clear();
            foreach (TourLog tourLog in tourLogs) {
                _tourLogs.Add(tourLog);
            }
            OnPropertyChanged(nameof(TourLogs));
            HasNoResults = _tourLogs.Count == 0;
        }

        public void Clear()
        {
            _selectedTourLog = null;
            _tourLogs.Clear();
        }
    }
}