using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TourPlanner.Models.Entities;
using TourPlanner.BL.Services;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels;

public class TourListViewModel : BaseViewModel {

    private readonly EventAggregator _eventAggregator;
    private readonly TourService _tourService;
    private bool _hasNoResults;
    private string _searchQuery;
    private Tour _selectedTour;
    private ObservableCollection<Tour> _tours;

    private double? _minPopularity;
    private bool? _childFriendliness;

    public TourListViewModel(EventAggregator eventAggregator, TourService tourService) {
        _eventAggregator = eventAggregator;
        _tourService = tourService;
        _tours = new ObservableCollection<Tour>();

        // Old search (string only)
        _eventAggregator.Subscribe<string>(query => {
            SearchQuery = query;
            MinPopularity = null;
            ChildFriendliness = null;
        });
        // New search tuple (advanced parameters)
        _eventAggregator.Subscribe<(string, double?, bool?)>(tuple => {
            SearchQuery = tuple.Item1;
            MinPopularity = tuple.Item2;
            ChildFriendliness = tuple.Item3;
        });
    }

    public Tour SelectedTour
    {
        get => _selectedTour;
        set {
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

    public double? MinPopularity
    {
        get => _minPopularity;
        set {
            if (_minPopularity == value) return;
            _minPopularity = value;
            OnPropertyChanged();
            PerformSearch();
        }
    }

    public bool? ChildFriendliness
    {
        get => _childFriendliness;
        set {
            if (_childFriendliness == value) return;
            _childFriendliness = value;
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
        // Use advanced search if attribute params are provided
        if (string.IsNullOrWhiteSpace(SearchQuery) && !MinPopularity.HasValue && !ChildFriendliness.HasValue) {
            ReloadTours(_tourService.GetAllTours().ToList());
            HasNoResults = false;
            return;
        }

        IEnumerable<Tour> tours;
        IEnumerable<TourLog> logs;

        if (MinPopularity.HasValue || ChildFriendliness.HasValue) {
            (tours, logs) = _tourService.SearchToursAndLogs(SearchQuery, MinPopularity, ChildFriendliness);
        }
        else {
            (tours, logs) = _tourService.SearchToursAndLogs(SearchQuery);
        }
        ReloadTours(tours.ToList());
        HasNoResults = !tours.Any() && !logs.Any();
    }
}