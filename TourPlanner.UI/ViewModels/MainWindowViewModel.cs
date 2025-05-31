using System;
using System.Windows.Controls;
using System.Windows.Input;
using TourPlanner.BL.Services;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;
using TourPlanner.UI.Views;

namespace TourPlanner.UI.ViewModels;

public class MainWindowViewModel : BaseViewModel {

    private readonly EventAggregator _eventAggregator;
    private readonly TourLogListViewModel _tourLogListViewModel;
    private readonly TourService _tourService;
    private UserControl _currentView;
    private UserControl _homeView;
    
    public MainWindowViewModel(
        HomeViewModel homeViewModel,
        TourManagementViewModel tourManagementViewModel,
        CreateTourLogViewModel createTourLogViewModel,
        CreateTourViewModel createTourViewModel,
        TourListViewModel tourListViewModel,
        TourLogListViewModel tourLogListViewModel,
        SearchBarViewModel searchBarViewModel,
        EventAggregator eventAggregator,
        TourService tourService
    ) {
        TourManagementViewModel = tourManagementViewModel;
        HomeViewModel = homeViewModel;
        CreateTourLogViewModel = createTourLogViewModel;
        CreateTourViewModel = createTourViewModel;
        TourListViewModel = tourListViewModel;
        _tourLogListViewModel = tourLogListViewModel;
        SearchBarViewModel = searchBarViewModel;
        _eventAggregator = eventAggregator;
        _tourService = tourService;

        _homeView = new Home {
            DataContext = HomeViewModel
        };

        CurrentView = new TourManagement {
            DataContext = TourManagementViewModel
        };

        _eventAggregator.Subscribe<string>(searchText =>
        {
            if (_tourLogListViewModel != null) {
                _tourLogListViewModel.SearchQuery = searchText;
            }
        });

        _eventAggregator.Subscribe<string>(NavigationHandler);
        ShowHomeView();
    }

    public HomeViewModel HomeViewModel { get; }
    public TourManagementViewModel TourManagementViewModel { get; }
    public CreateTourLogViewModel CreateTourLogViewModel { get; }
    public CreateTourViewModel CreateTourViewModel { get; }
    public TourListViewModel TourListViewModel { get; }
    public SearchBarViewModel SearchBarViewModel { get; }

    public UserControl CurrentView
    {
        get => _currentView;
        set {
            _currentView = value;
            OnPropertyChanged();
        }
    }

    public UserControl HomeView
    {
        get => _homeView;
        set {
            _homeView = value;
            OnPropertyChanged();
        }
    }

    private void NavigationHandler(string message) {
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

    private void ShowHomeView() {
        CurrentView = _homeView;
    }

    private void ShowCreateTour() {
        CurrentView = new CreateTour {
            DataContext = CreateTourViewModel
        };
    }

    private void ShowCreateTourLog() {
        CurrentView = new CreateTourLog {
            DataContext = CreateTourLogViewModel
        };
    }
}