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
    public class TourViewModel : INotifyPropertyChanged
    {
        public TourViewModel()
        {

        }

        public ObservableCollection<Tour> Tours { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void AddTour (Tour tour)
        {

        }
    }
}
