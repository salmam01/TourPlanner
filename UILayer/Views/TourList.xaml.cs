using System.Windows.Controls;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner.UILayer.Views;

public partial class TourList : UserControl {
    public TourList() {
        InitializeComponent();

        if (DataContext is TourListViewModel viewModel) {
            // SearchBar.DataContext = new SearchBarViewModel();
            // var searchViewModel = (SearchBarViewModel)SearchBar.DataContext;
            // searchViewModel.SearchTextChanged += (sender, query) =>
            // {
            //     viewModel.SearchQuery = query;
            // };
        }
    }
}