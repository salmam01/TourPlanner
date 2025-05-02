using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            set
            {
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
            set
            {
                _tourLogs = value;
                OnPropertyChanged(nameof(TourLogs));
            }
        }

        public EventHandler<TourLog> TourLogSelected;

        public TourLogListViewModel()
        {
            _tourLogs = new ObservableCollection<TourLog>();

            Console.WriteLine("TourLogsListViewModel INITIALIZED");
        }

        public void OnTourLogCreated(TourLog tourLog)
        {
            if(tourLog == null)
            { 
                Console.WriteLine("Tour Log is null!");
                return;
            }

            _tourLogs.Add(tourLog);
            Console.WriteLine(_tourLogs.Count());

            Console.WriteLine("Tour Log added to Tour Log List!\nList:\n");
            foreach (TourLog tl in _tourLogs)
            {
                Console.WriteLine($"{tl.Comment} on the {tl.Date}\n");
            }
        }

        public void OnTourLogUpdated(TourLog tourLog)
        {
            int i = 0;
            foreach (TourLog tl in _tourLogs) {
                if (tl.Id == _selectedTourLog.Id)
                {
                    _tourLogs[i] = tourLog;
                    break;
                }
                i++;
            }
        }

        public void OnTourLogDeleted(TourLog tourLog)
        {
            if (tourLog == null)
            {
                Console.WriteLine("Tour Log is null!");
                return;
            }

            _tourLogs.Remove(tourLog);
            Console.WriteLine("Tour Log removed from Tour Log List!\nList:\n");
        }
    }
}
