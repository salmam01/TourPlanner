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
            execute => OnUpdateTour()
        );

        public ICommand ExportToursCommand => new RelayCommand(
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

            _eventAggregator.Subscribe<Tour>(ShowTourDetails);
        }

        public void SetDefaultValues()
        {
            Name = "Select a Tour";

            Date = DateTime.Now.ToString("dd-MM-yyyy");
            Description = "";

            From = "";
            To = "";
            TransportType = "";
            Distance = "0 km";
            EstimatedTime = $"{TimeSpan.Zero.ToString(@"hh\:mm")} h";

            Popularity = 0;
            ChildFriendly = "No";
        }

        public void ShowTourDetails(Tour tour)
        {
            if (tour == null) return;
            if (tour == _selectedTour) return;
            _selectedTour = tour;

            Name = tour.Name;
            Date = tour.Date.ToString("dd-MM-yyyy");
            Description = tour.Description;

            From = tour.From;
            To = tour.To;
            TransportType = tour.TransportType;

            double distanceInKm = tour.Distance / 1000;
            Distance = $"{distanceInKm.ToString("F2")} km";
            EstimatedTime = $"{tour.EstimatedTime.ToString(@"hh\:mm")} h";

            Popularity = tour.TourAttributes.Popularity;
            ChildFriendly = tour.TourAttributes.ChildFriendliness ? "Yes" : "No";
        }

        public void OnUpdateTour()
        {
            _eventAggregator.Publish(new UpdateTourEvent());
        }

        public void OnExportTour()
        {
            _eventAggregator.Publish(new ExportTourEvent());
        }

        public void OnDeleteTour()
        {
            _eventAggregator.Publish(new DeleteTourEvent());
        }
    }
}
