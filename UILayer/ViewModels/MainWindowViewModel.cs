using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Views;

namespace TourPlanner.UILayer.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private UserControl _currentView;
        public RelayCommand ShowCreateTourCommand => new RelayCommand(execute => ShowCreateTour());
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public MainWindowViewModel()
        {
            CurrentView = new Home();
        }
        
        private void ShowCreateTour()
        {
            CurrentView = new CreateTour();
        }

        private bool CanButtonClick()
        {
            return true;
        }
    }
}
