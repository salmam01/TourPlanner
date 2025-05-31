using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels;

public class SearchBarViewModel : INotifyPropertyChanged {
    public SearchBarViewModel() {
        SearchCommand = new RelayCommand(ExecuteSearch);
    }

    private string _searchText;
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

    public ICommand SearchCommand { get; }
    public event PropertyChangedEventHandler PropertyChanged;
    public event EventHandler<string> SearchParamsChanged;

    private void ExecuteSearch(object obj) {
        OnSearchParamsChanged();
    }

    private void OnSearchParamsChanged()
    {
        SearchParamsChanged?.Invoke(this, SearchText);
    }

    protected void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}