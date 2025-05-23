using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels;

public class SearchBarViewModel : INotifyPropertyChanged {

    private readonly EventAggregator _eventAggregator;
    private string _searchText;

    private double? _minPopularity;
    private bool? _childFriendliness;

    public SearchBarViewModel(EventAggregator eventAggregator) {
        _eventAggregator = eventAggregator;
        SearchCommand = new RelayCommand(ExecuteSearch);
    }

    public string SearchText
    {
        get => _searchText;
        set {
            if (_searchText == value) return;
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            OnSearchParamsChanged();
        }
    }

    public double? MinPopularity
    {
        get => _minPopularity;
        set {
            if (_minPopularity == value) return;
            _minPopularity = value;
            OnPropertyChanged(nameof(MinPopularity));
            OnSearchParamsChanged();
        }
    }

    public bool? ChildFriendliness
    {
        get => _childFriendliness;
        set {
            if (_childFriendliness == value) return;
            _childFriendliness = value;
            OnPropertyChanged(nameof(ChildFriendliness));
            OnSearchParamsChanged();
        }
    }

    public ICommand SearchCommand { get; }
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<(string SearchText, double? MinPopularity, bool? ChildFriendliness)> SearchParamsChanged;
    public event EventHandler<string> SearchTextChanged; // Legacy, avoid if possible

    private void ExecuteSearch(object obj) {
        OnSearchParamsChanged();
    }

    private void OnSearchParamsChanged()
    {
        SearchParamsChanged?.Invoke(this, (SearchText, MinPopularity, ChildFriendliness));
        SearchTextChanged?.Invoke(this, _searchText); // Legacy / fallback
        _eventAggregator.Publish((SearchText, MinPopularity, ChildFriendliness));
    }

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}