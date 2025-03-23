using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Stores;
using TourPlanner.UILayer.Views;

namespace TourPlanner.UILayer.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private TourManagementViewModel _tourManagementVm;
        private TourListViewModel _tourListVm;

        private Tour _selectedTour;
        public Tour SelectedTour
        {
            get => _selectedTour;
            set
            {
                _selectedTour = value;
                OnPropertyChanged(nameof(SelectedTour));
            }
        }

        public ICommand ModifyTourCommand;
        public ICommand DeleteTourCommand;

        public HomeViewModel(CreateTourViewModel createTourViewModel)
        {
            _tourManagementVm = new TourManagementViewModel();
            _tourListVm = new TourListViewModel(createTourViewModel);

            ModifyTourCommand = new RelayCommand(execute => ModifyTour());
            DeleteTourCommand = new RelayCommand(execute => DeleteTour());
        }

        public void ModifyTour()
        {

        }

        public void DeleteTour()
        {

        }

    }
}
