using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Stores;

namespace TourPlanner.UILayer.ViewModels
{
    public class TourListViewModel : BaseViewModel
    {
        private ObservableCollection<Tour> _tours;
        public ObservableCollection<Tour> Tours
        {
            get => _tours;
            set
            {
                _tours = value;
                OnPropertyChanged(nameof(Tour));
            }
        }

        public TourListViewModel(CreateTourViewModel createTourViewModel)
        {
            _tours = new ObservableCollection<Tour>();
            _tours.Add(new Tour("Bosnia Roadtrip", new DateTime(2025, 4, 20), "A roadtrip through Bosnia.", "Sarajevo", "Srebrenica"));

            createTourViewModel.TourCreated += OnTourCreated;
        }

        private void OnTourCreated(Tour tour)
        {
            _tours.Add(tour);
        }
    }
}
