using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
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

        public ICommand ShowHomeViewCommand { get; }
        public ICommand ShowCreateTourCommand { get; }
        public ICommand ShowCreateTourLogCommand { get; }


        public MainWindowViewModel(HomeViewModel homeViewModel, CreateTourViewModel createTourViewModel, CreateTourLogViewModel createTourLogViewModel)
        {
            _createTourViewModel = createTourViewModel;
            _homeViewModel = homeViewModel;
            _createTourLogViewModel = createTourLogViewModel;

            _homeView = new Home
            {
                DataContext = _homeViewModel
            };

            ShowHomeViewCommand = new RelayCommand(execute => ShowHomeView());
            ShowCreateTourCommand = new RelayCommand(execute => ShowCreateTour());
            ShowCreateTourLogCommand = new RelayCommand(execute => ShowCreateTourLog());

            //_homeViewModel.CreateTourLog += ShowCreateTourLog;

            ShowHomeView();
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
            CurrentView = new CreateTourLog
            {
                DataContext = _createTourLogViewModel
            };
        }
    }
}