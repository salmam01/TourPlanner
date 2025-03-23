using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourListViewModel : BaseViewModel
    {
        private Tour _selectedTour;
        public ObservableCollection<Tour> Tours { get; set; }

        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                if (_selectedTour == value) return;
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }

       // public event EventHandler<Tour> SelectedTourChanged;

        public TourListViewModel()
        {
            Tours = new ObservableCollection<Tour>();
            // aample data for testing
            Tour tour = new Tour("Test Tour", DateTime.Now.ToString(), "Test Description", "Vienna", "Salzburg")
            {
                Id = Guid.NewGuid(),
                TourLogs = new List<TourLog>()
            };
            Tours.Add(tour);
        }
    }
}
