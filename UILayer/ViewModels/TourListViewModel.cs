using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.BusinessLayer.Services;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels;

public class TourListViewModel : BaseViewModel {

    private readonly EventAggregator _eventAggregator;
    private readonly TourService _tourService;
    private bool _hasNoResults;
    private string _searchQuery;
    private Tour _selectedTour;
    private ObservableCollection<Tour> _tours;

    public TourListViewModel(EventAggregator eventAggregator, TourService tourService) {
        _eventAggregator = eventAggregator;
        _tourService = tourService;
        _eventAggregator.Subscribe<string>(query => { SearchQuery = query; });
        _tours = new ObservableCollection<Tour>();
    }

    public Tour SelectedTour
    {
        get => _selectedTour;
        set
        {
            if (_selectedTour == value) return;
            _selectedTour = value;
            OnPropertyChanged();
            _eventAggregator.Publish(_selectedTour);
        }
    }

    public ObservableCollection<Tour> Tours
    {
        get => _tours;
        set
        {
            _tours = value;
            OnPropertyChanged();
        }
    }

    public string SearchQuery
    {
        get => _searchQuery;
        set
        {
            if (_searchQuery == value) return;
            _searchQuery = value;
            OnPropertyChanged();
            PerformSearch();
        }
    }

    public bool HasNoResults
    {
        get => _hasNoResults;
        set
        {
            if (_hasNoResults == value) return;
            _hasNoResults = value;
            OnPropertyChanged();
        }
    }

    public void ReloadTours(List<Tour> tours) {
        _tours.Clear();
        foreach (Tour tour in tours) {
            _tours.Add(tour);
        }
        HasNoResults = _tours.Count == 0;
        Console.WriteLine("Tour List reloaded.");
    }

    public void OnTourUpdated(Tour updatedTour) {
        var i = 0;
        foreach (Tour tour in _tours) {
            if (tour.Id == _selectedTour.Id) {
                _tours[i].Name = updatedTour.Name;
                _tours[i].Date = updatedTour.Date;
                _tours[i].Description = updatedTour.Description;
                _tours[i].TransportType = updatedTour.TransportType;
                _tours[i].From = updatedTour.From;
                _tours[i].To = updatedTour.To;
                break;
            }
            i++;
        }
    }

    private void PerformSearch() {
        if (string.IsNullOrWhiteSpace(SearchQuery)) {
            ReloadTours(_tourService.GetAllTours().ToList());
            HasNoResults = false;
            return;
        }

        (IEnumerable<Tour> tours, IEnumerable<TourLog> logs) = _tourService.SearchToursAndLogs(SearchQuery);
        ReloadTours(tours.ToList());
        HasNoResults = !tours.Any() && !logs.Any();
    }
}