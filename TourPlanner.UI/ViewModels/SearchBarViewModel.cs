using System;
using System.ComponentModel;
using System.Windows.Input;
using TourPlanner.UI.Commands;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels;

public class SearchBarViewModel : BaseViewModel {

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

    public event EventHandler<string> SearchParamsChanged;

    private void OnSearchParamsChanged()
    {
        SearchParamsChanged?.Invoke(this, SearchText);
    }
}