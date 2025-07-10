using System;
using System.Windows.Controls;
using TourPlanner.UI.Events;
using TourPlanner.UI.Views;

namespace TourPlanner.UI.ViewModels;

public class MainWindowViewModel : BaseViewModel {

    private readonly EventAggregator _eventAggregator;
    private UserControl _currentView;
    private UserControl _homeView;

    public HomeViewModel HomeViewModel { get; }
    public CreateTourLogViewModel CreateTourLogViewModel { get; }
    public CreateTourViewModel CreateTourViewModel { get; }
    public LogViewerViewModel LogViewerViewModel { get; }

    public MainWindowViewModel(
        HomeViewModel homeViewModel,
        CreateTourLogViewModel createTourLogViewModel,
        CreateTourViewModel createTourViewModel,
        LogViewerViewModel logViewerViewModel,
        EventAggregator eventAggregator
    ) {
        HomeViewModel = homeViewModel;
        CreateTourLogViewModel = createTourLogViewModel;
        CreateTourViewModel = createTourViewModel;
        LogViewerViewModel = logViewerViewModel;
        _eventAggregator = eventAggregator;

        _homeView = new Home {
            DataContext = HomeViewModel
        };

        _eventAggregator.Subscribe<NavigationEvent>(NavigationHandler);
        ShowHomeView();
    }

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

    private void NavigationHandler(NavigationEvent navigationEvent) {
        switch (navigationEvent.Destin) {
            case NavigationEvent.Destination.Home:
                ShowHomeView();
                break;
            case NavigationEvent.Destination.CreateTour:
                ShowCreateTour();
                break;
            case NavigationEvent.Destination.CreateTourLog:
                ShowCreateTourLog();
                break;
            case NavigationEvent.Destination.LogViewer:
                ShowLogViewer();
                break;
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

    private void ShowLogViewer()
    {
        CurrentView = new LogViewer
        {
            DataContext = LogViewerViewModel
        };
    }
}