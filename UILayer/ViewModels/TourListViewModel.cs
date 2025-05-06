using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourListViewModel : BaseViewModel
    {
        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set {
                if (_selectedTour == value) return;
                _selectedTour = value;
               OnPropertyChanged(nameof(SelectedTour));
                _eventAggregator.Publish(_selectedTour);
            }
        }

        private ObservableCollection<Tour> _tours;
        public ObservableCollection<Tour> Tours
        {
            get => _tours;
            set {
                _tours = value;
                OnPropertyChanged(nameof(Tours));
            }
        }

        private readonly EventAggregator _eventAggregator;

        public TourListViewModel(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _tours = new ObservableCollection<Tour>();
        }

        public void ReloadTours(List<Tour> tours)
        {
            _tours.Clear();
            foreach (Tour tour in tours)
            {
                _tours.Add(tour);
            }
            Console.WriteLine("Tour List reloaded.");
        }

        public void OnTourUpdated(Tour updatedTour)
        {
            int i = 0;
            foreach (Tour tour in _tours)
            {
                if (tour.Id == _selectedTour.Id)
                {
                    _tours[i].Name = updatedTour.Name;
                    _tours[i].Date = updatedTour.Date;
                    _tours[i].Description = updatedTour.Description;
                    _tours[i].TransportType = updatedTour.TransportType;
                    _tours[i].From = updatedTour.From;
                    _tours[i].To = updatedTour.To;
                    break;
                }
                i++;
            }
        }
    }
}
