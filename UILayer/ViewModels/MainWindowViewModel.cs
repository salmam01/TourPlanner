using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;

namespace TourPlanner.UILayer.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Tour> Tours { get; set; }
        public RelayCommand AddTourCommand => new RelayCommand(execute => AddTour());
        public RelayCommand RemoveTourCommand => new RelayCommand(execute => RemoveTour());

        public MainWindowViewModel()
        {
            Tours = new ObservableCollection<Tour>();
        }

        public void AddTour()
        {
            
        }

        public void RemoveTour()
        {

        }

        private bool CanButtonClick()
        {
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
