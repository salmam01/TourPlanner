using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourDetailsViewModel: BaseViewModel
    {
        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;

        public ICommand EditTourCommand => new RelayCommand(
            execute => OnEditTour()
        );

        public ICommand ExportTourCommand => new RelayCommand(
            execute => OnExportTour()
        );

        public ICommand DeleteTourCommand => new RelayCommand(
            execute => OnDeleteTour()
        );

        private string _name;
        public string Name
        {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private string _date;
        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        private string _from;
        public string From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged(nameof(From));
            }
        }

        private string _to;
        public string To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged(nameof(To));
            }
        }

        private string _transportType;
        public string TransportType
        {
            get => _transportType;
            set
            {
                _transportType = value;
                OnPropertyChanged(nameof(TransportType));
            }
        }

        private string _distance;
        public string Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

        private string _estimatedTime;
        public string EstimatedTime
        {
            get => _estimatedTime;
            set
            {
                _estimatedTime = value;
                OnPropertyChanged(nameof(EstimatedTime));
            }
        }

        private double _popularity;
        public double Popularity
        {
            get => _popularity;
            set
            {
                _popularity = value;
                OnPropertyChanged(nameof(Popularity));
            }
        }

        private string _childFriendly;
        public string ChildFriendly
        {
            get => _childFriendly;
            set
            {
                _childFriendly = value;
                OnPropertyChanged(nameof(ChildFriendly));
            }
        }

        public TourDetailsViewModel(EventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            SetDefaultValues();

            _eventAggregator.Subscribe<TourEvent>(e =>
            {
                EventHandler(e);
            });
        }

        private void SetDefaultValues()
        {
            Name = "Select a Tour";

            Date = DateTime.Now.ToString("dd-MM-yyyy");
            Description = "";

            From = "";
            To = "";
            TransportType = "";
            Distance = "0 km";
            EstimatedTime = $"{TimeSpan.Zero.ToString(@"hh\:mm\:ss")} h";

            Popularity = 0;
            ChildFriendly = "No";

            _selectedTour = null;
        }

        private void ShowTourDetails(Tour tour)
        {
            if (tour == null) return;
            _selectedTour = tour;

            Name = tour.Name;
            Date = tour.Date.ToString("dd-MM-yyyy");
            Description = tour.Description;

            From = tour.From;
            To = tour.To;
            TransportType = tour.TransportType;

            double distanceInKm = tour.Distance / 1000;
            Distance = $"{distanceInKm.ToString("F2")} km";
            EstimatedTime = $"{tour.EstimatedTime.ToString(@"hh\:mm\:ss")} h";

            Popularity = tour.TourAttributes.Popularity;
            ChildFriendly = tour.TourAttributes.ChildFriendliness ? "Yes" : "No";
        }

        private void EventHandler(TourEvent tourEvent)
        {
            switch (tourEvent.Type)
            {
                case TourEvent.EventType.Reload:
                    SetDefaultValues();
                    break;
                case TourEvent.EventType.SelectTour:
                    ShowTourDetails(tourEvent.Tour);
                    break;
                case TourEvent.EventType.TourEdited:
                    ShowTourDetails(tourEvent.Tour);
                    break;
                case TourEvent.EventType.TourDeleted:
                    SetDefaultValues();
                    break;
                case TourEvent.EventType.LogCreated:
                    ShowTourDetails(tourEvent.Tour);
                    break;
                case TourEvent.EventType.AllToursDeleted:
                    SetDefaultValues();
                    break;
            }
        }

        private void OnEditTour()
        {
            _eventAggregator.Publish(new TourEvent(TourEvent.EventType.EditTour));
        }

        private void OnExportTour()
        {
            _eventAggregator.Publish(new TourEvent(TourEvent.EventType.ExportTour));
        }

        private void OnDeleteTour()
        {
            _eventAggregator.Publish(new TourEvent(TourEvent.EventType.DeleteTour));
        }
    }
}
