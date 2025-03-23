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
using TourPlanner.UILayer.Stores;

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

        public ICommand CreateTourCommand { get; }
        public event Action<Tour> TourCreated;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = value;
                OnPropertyChanged(nameof(Date));
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string TransportType
        {
            get
            {
                return _transportType;
            }
            set
            {
                _transportType = value;
                OnPropertyChanged(nameof(TransportType));
            }
        }

        public string From
        {
            get
            {
                return _from;
            }
            set
            {
                _from = value;
                OnPropertyChanged(nameof(From));
            }
        }

        public string To
        {
            get
            {
                return _to;
            }
            set
            {
                _to = value;
                OnPropertyChanged(nameof(To));
            }
        }

        public CreateTourViewModel()
        {
            CreateTourCommand = new RelayCommand(execute => CreateTour());
        }

        public void CreateTour()
        {
            Tour tour = new Tour(_name, _date, _description, _from, _to);
            TourCreated?.Invoke(tour);

            MessageBox.Show(tour.Name + " has been created");
        }
    }
}
