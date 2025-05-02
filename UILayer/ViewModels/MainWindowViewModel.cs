using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Events;
using TourPlanner.UILayer.Views;

namespace TourPlanner.UILayer.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private HomeViewModel _homeViewModel;
        public HomeViewModel HomeViewModel => _homeViewModel;

        private CreateTourViewModel _createTourViewModel;
        public CreateTourViewModel TourManagementViewModel => _createTourViewModel;

        private CreateTourLogViewModel _createTourLogViewModel;
        public CreateTourLogViewModel CreateTourLogViewModel => _createTourLogViewModel;

        private readonly EventAggregator _eventAggregator;

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }
        private UserControl _homeView;
        public UserControl HomeView
        {
            get => _homeView;
            set
            {
                _homeView = value;
                OnPropertyChanged(nameof(HomeView));
            }
        }

        public ICommand ShowHomeViewCommand => new RelayCommand(
            execute => ShowHomeView()
        );
        public ICommand ShowCreateTourCommand => new RelayCommand(
            execute => ShowCreateTour()
        );
        public ICommand ShowCreateTourLogCommand => new RelayCommand(
            execute => ShowCreateTourLog()
        );


        public MainWindowViewModel(
            HomeViewModel homeViewModel,
            CreateTourViewModel createTourViewModel,
            CreateTourLogViewModel createTourLogViewModel,
            EventAggregator eventAggregator
        ) {
            _createTourViewModel = createTourViewModel;
            _homeViewModel = homeViewModel;
            _createTourLogViewModel = createTourLogViewModel;
            _eventAggregator = eventAggregator;

            _homeView = new Home
            {
                DataContext = _homeViewModel
            };

            _eventAggregator.Subscribe<string>(NavigationHandler);
            ShowHomeView();
        }

        private void NavigationHandler(string message)
        {
            switch (message) {
                case "ShowHome":
                    ShowHomeView();
                    break;

                case "ShowCreateTour":
                    ShowCreateTour();
                    break;

                case "ShowCreateTourLog":
                    ShowCreateTourLog();
                    break;

                default:
                    //"No such view found"
                    return;
            }
        }

        private void ShowHomeView()
        {
            CurrentView = _homeView;
        }

        private void ShowCreateTour()
        {
            CurrentView = new CreateTour
            {
                DataContext = _createTourViewModel
            };
        }

        private void ShowCreateTourLog()
        {
            Console.WriteLine("ShowCreateTourLog called");
            CurrentView = new CreateTourLog
            {
                DataContext = _createTourLogViewModel
            };
        }
    }
}