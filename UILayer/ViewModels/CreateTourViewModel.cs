using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;

namespace TourPlanner.UILayer.ViewModels
{
    public class CreateTourViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _date;
        private string _description;
        private string _transportType;
        private string _from;
        private string _to;

        public RelayCommand CreateTourCommand => new RelayCommand(execute => CreateTour());

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

        public string Date
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

        public void CreateTour()
        {
            Tour tour = new Tour(_name, _date, _description, _from, _to);

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
