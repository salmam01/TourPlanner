using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels;

public class TourListViewModel : BaseViewModel {

    private readonly EventAggregator _eventAggregator;
    
    private Tour _selectedTour;
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

    private ObservableCollection<Tour> _tours;
    public ObservableCollection<Tour> Tours
    {
        get => _tours;
        set
        {
            _tours = value;
            OnPropertyChanged();
        }
    }

    private bool _hasNoResults;
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

    public TourListViewModel(
        EventAggregator eventAggregator
    )
    {
        _eventAggregator = eventAggregator;
        _tours = new ObservableCollection<Tour>();
    }

    public void ReloadTours(List<Tour> tours) {
        _tours.Clear();
        foreach (Tour tour in tours) {
            _tours.Add(tour);
        }
        HasNoResults = _tours.Count == 0;
    }
}