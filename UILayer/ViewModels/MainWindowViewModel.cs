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
        private UserControl _homeView;
        public ICommand ShowCreateTourCommand { get; }
        public ICommand ShowHomeViewCommand { get; }
        public ObservableCollection<Tour> Tours { get; set; } = new ObservableCollection<Tour>();

        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public UserControl HomeView
        {
            get => _homeView;
            set
            {
                _homeView = value;
                OnPropertyChanged(nameof(HomeView));
            }
        }

        public MainWindowViewModel()
        {
            ShowCreateTourCommand = new RelayCommand(execute => ShowCreateTour());
            ShowHomeViewCommand = new RelayCommand(execute => ShowHomeView());

            _homeView = new Home();
            CurrentView = _homeView;
        }
        
        private void ShowCreateTour()
        {
            CurrentView = new CreateTour();
        }

        private void ShowHomeView()
        {
            CurrentView = _homeView;
        }

        private bool CanButtonClick()
        {
            return true;
        }
    }
}
