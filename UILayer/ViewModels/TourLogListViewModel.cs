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
            TourLogs = new ObservableCollection<TourLog>();

            Console.WriteLine("TourLogsListViewModel INITIALIZED");
        }

        public void OnTourLogCreated(TourLog tourLog)
        {
            if(tourLog == null) 
            { 
                Console.WriteLine("Tour Log is null!");
                return;
            }

            TourLogs.Add(tourLog);

            Console.WriteLine("Tour Log added to Tour Log List!\nList:\n");
            foreach (TourLog tl in TourLogs)
            {
                Console.WriteLine($"{tl.Comment} on the {tl.Date}\n");
            }
        }

        public void OnTourLogEdited(TourLog tourLog)
        {
            //  TODO: implement
            foreach (TourLog tl in _tourLogs) {
                if (tl.Id == tourLog.Id)
                {
                    //tl = tourLog; ?? idk what to do here
                }
            }
        }

        public void OnTourLogDeleted(TourLog tourLog)
        {
            if (tourLog == null)
            {
                Console.WriteLine("Tour Log is null!");
                return;
            }

            TourLogs.Remove(tourLog);
            Console.WriteLine("Tour Log removed from Tour Log List!\nList:\n");
        }
    }
}
