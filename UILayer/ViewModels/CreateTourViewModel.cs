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

        public ICommand CreateTourCommand => new RelayCommand(execute => CreateTour(), canExecute => CanCreate);

        public CreateTourViewModel()
        {
            //  TODO: check if this assignment is necessary and if you should be using the private or public properties
            _name = "";
            _date = DateTime.Now;
            _description = "";
            _from = "";
            _to = "";
        }

        private void CreateTour()
        {
            Tour tour = new Tour(
                _name,
                _date,
                _description,
                _transportType, //  maybe better to use character ? 
                _from,
                _to);

            TourCreated?.Invoke(this, tour);
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
