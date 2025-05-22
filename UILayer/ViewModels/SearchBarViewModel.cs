using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.UILayer.Commands;
using TourPlanner.UILayer.Events;

namespace TourPlanner.UILayer.ViewModels;

public class SearchBarViewModel : INotifyPropertyChanged {

    private readonly EventAggregator _eventAggregator;
    private string _searchText;

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
            Console.WriteLine($"Searching for: {value}");
            OnPropertyChanged(nameof(SearchText));
            SearchTextChanged?.Invoke(this, _searchText);
            _eventAggregator.Publish(_searchText);
        }
    }

    public ICommand SearchCommand { get; }
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<string> SearchTextChanged;

    private void ExecuteSearch(object obj) {
        _eventAggregator.Publish(_searchText);
    }

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}