using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourLogListViewModel : BaseViewModel
    {
        private TourLog _selectedTourLog;
        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set {
                if(_selectedTourLog == value) return;
                _selectedTourLog = value;
                OnPropertyChanged(nameof(SelectedTourLog));
                TourLogSelected?.Invoke(this, _selectedTourLog);
            }
        }

        private ObservableCollection<TourLog> _tourLogs;
        public ObservableCollection<TourLog> TourLogs
        {
            get => _tourLogs;
            set {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        public EventHandler<TourLog> TourLogSelected;

        public TourLogListViewModel()
        {
            _tourLogs = new ObservableCollection<TourLog>();
        }

        public void ReloadTourLogs(IEnumerable<TourLog> tourLogs)
        {
            _tourLogs.Clear();
            foreach (TourLog tourLog in tourLogs)
            {
                _tourLogs.Add(tourLog);
            }
            Console.WriteLine("TourLog List reloaded.");
        }
    }
}
