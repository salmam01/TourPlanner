using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class TourDetailsViewModel: BaseViewModel
    {
        private readonly EventAggregator _eventAggregator;
        private Tour _selectedTour;

        private string _name;
        public string Name
        {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private DateTime _date;
        public DateTime Date
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

        private double _distance;
        public double Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                OnPropertyChanged(nameof(Distance));
            }
        }

        private TimeSpan _estimatedTime;
        public TimeSpan EstimatedTime
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
            Date = DateTime.Now;
            Description = "";

            From = "";
            To = "";
            TransportType = "";
            Distance = 0;
            EstimatedTime = TimeSpan.Zero;

            Popularity = 0;
            ChildFriendly = "No";
        }

        public void ShowTourDetails(Tour tour)
        {
            if (tour == null) return;
            if (tour == _selectedTour) return;
            _selectedTour = tour;

            Name = tour.Name;
            Date = tour.Date;
            Description = tour.Description;

            From = tour.From;
            To = tour.To;
            TransportType = tour.TransportType;
            Distance = tour.Distance;
            EstimatedTime = tour.EstimatedTime;

            Popularity = tour.TourAttributes.Popularity;
            ChildFriendly = tour.TourAttributes.ChildFriendliness ? "Yes" : "No";

        }
    }
}
