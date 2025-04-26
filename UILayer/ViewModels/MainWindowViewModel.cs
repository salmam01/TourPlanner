using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Views;

namespace TourPlanner.UILayer.ViewModels
{
    public class MainWindowViewModel : BaseViewModel
    {
        private CreateTourViewModel _createTourViewModel;
        public CreateTourViewModel TourManagementViewModel => _createTourViewModel;

        private HomeViewModel _homeViewModel;
        public HomeViewModel HomeViewModel => _homeViewModel;

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

        public ICommand ShowCreateTourCommand { get; }
        public ICommand ShowHomeViewCommand { get; }


        public MainWindowViewModel(HomeViewModel homeViewModel, CreateTourViewModel createTourViewModel)
        {
            _createTourViewModel = createTourViewModel;
            _homeViewModel = homeViewModel;

            _homeView = new Home
            {
                DataContext = _homeViewModel
            };

            ShowCreateTourCommand = new RelayCommand(execute => ShowCreateTour());
            ShowHomeViewCommand = new RelayCommand(execute => ShowHomeView());
            ShowHomeView();
        }
        
        private void ShowCreateTour()
        {
            CurrentView = new CreateTour
            {
                DataContext = _createTourViewModel
            };
        }

        private void ShowHomeView()
        {
            CurrentView = _homeView;
        }
    }
}