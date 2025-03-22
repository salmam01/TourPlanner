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
        public ObservableCollection<Tour> Tours { get; set; }

        public TourListViewModel()
        {
            Tours = new ObservableCollection<Tour>();
        }
    }
}
