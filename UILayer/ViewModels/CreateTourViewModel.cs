using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;

namespace TourPlanner.UILayer.ViewModels
{
    public class CreateTourViewModel : BaseViewModel
    {
        private string _name;
        private DateTime _date;
        private string _description;
        private string _transportType;
        private string _from;
        private string _to;

        public EventHandler<Tour> TourCreated;
        public EventHandler<Tour> TourUpdated;
        public bool _isEditing = false;
        public string SubmitButtonText => _isEditing ? "Save Tour" : "Create Tour";

        public bool CanCreate => ValidateInput();

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string TransportType
        {
            get => _transportType;
            set
            {
                _transportType = value;
                OnPropertyChanged(nameof(TransportType));
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string From
        {
            get => _from;
            set
            {
                _from = value;
                OnPropertyChanged(nameof(From));
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public string To
        {
            get => _to;
            set
            {
                _to = value;
                OnPropertyChanged(nameof(To));
                OnPropertyChanged(nameof(CanCreate));
            }
        }

        public List<string> TransportTypeOptions { get; } = new List<string>
        {
            "Plane",
            "Bus",
            "Train",
            "Car"
        };

        public ICommand CreateTourCommand => new RelayCommand(execute => CreateTour(), canExecute => CanCreate);

        public CreateTourViewModel()
        {
            ResetForm();
        }

        public void ResetForm()
        {
            _name = "";
            _date = DateTime.Now;
            _description = "";
            _from = "";
            _to = "";
            _isEditing = false;
            OnPropertyChanged(nameof(SubmitButtonText));
        }

        private void CreateTour()
        {
            Tour tour = new Tour(
                _name,
                _date,
                _description,
                _transportType,
                _from,
                _to
            );

            if(_isEditing)
            {
                TourUpdated?.Invoke(this, tour);
            }
            else
            {
                TourCreated?.Invoke(this, tour);
            }

            ResetForm();
        }

        public void LoadTour(Tour tour)
        {
            _name = tour.Name;
            _date = tour.Date;
            _description = tour.Description;
            _transportType = tour.TransportType;
            _from = tour.From;
            _to = tour.To;
            OnPropertyChanged(nameof(SubmitButtonText));
            _isEditing = true;
        }

        private bool ValidateInput()
        {
            // TODO: Add better validation
            return !string.IsNullOrEmpty(Name) &&
                   !string.IsNullOrEmpty(Description) &&
                   !string.IsNullOrEmpty(TransportType) &&
                   !string.IsNullOrEmpty(From) &&
                   !string.IsNullOrEmpty(To) &&
                   Date > DateTime.Now;
        }
    }
}
