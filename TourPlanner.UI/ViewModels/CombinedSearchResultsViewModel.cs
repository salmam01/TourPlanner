using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TourPlanner.Models.Entities;
using TourPlanner.BL.Services;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels;
public class CombinedSearchResultsViewModel : BaseViewModel {
    private readonly EventAggregator _eventAggregator;
    private readonly TourService _tourService;
    private bool _hasNoResults;
    private string _searchQuery;

    public CombinedSearchResultsViewModel(TourService tourService, EventAggregator eventAggregator) {
        _tourService = tourService;
        _eventAggregator = eventAggregator;

        Tours = new ObservableCollection<Tour>();
        Logs = new ObservableCollection<TourLog>();
        _eventAggregator.Subscribe<string>(query => { SearchQuery = query; });
    }

    public ObservableCollection<Tour> Tours { get; }
    public ObservableCollection<TourLog> Logs { get; }

    public string SearchQuery
    {
        get => _searchQuery;
        set {
            if (_searchQuery == value) return;
            _searchQuery = value;
            OnPropertyChanged();
            PerformSearch();
        }
    }

    public bool HasNoResults
    {
        get => _hasNoResults;
        set {
            if (_hasNoResults == value) return;
            _hasNoResults = value;
            OnPropertyChanged();
        }
    }

    private void PerformSearch() {
        if (string.IsNullOrWhiteSpace(SearchQuery)) {
            ReloadTours(_tourService.GetAllTours().ToList());
            ReloadLogs(Enumerable.Empty<TourLog>().ToList());
            HasNoResults = false;
            return;
        }

        (IEnumerable<Tour> tours, IEnumerable<TourLog> logs) = _tourService.SearchToursAndLogs(SearchQuery);
        ReloadTours(tours.ToList());
        ReloadLogs(logs.ToList());
        HasNoResults = !tours.Any() && !logs.Any();
    }

    private void ReloadTours(List<Tour> tours) {
        Tours.Clear();
        foreach (Tour tour in tours)
            Tours.Add(tour);
    }

    private void ReloadLogs(List<TourLog> logs) {
        Logs.Clear();
        foreach (TourLog log in logs)
            Logs.Add(log);
    }
}