using System.Windows;
using System.Windows.Controls;

namespace TourPlanner.UI.Views
{
    public partial class SearchResults : UserControl
    {
        public static readonly DependencyProperty ShowSearchBarProperty =
            DependencyProperty.Register(
                nameof(ShowSearchBar),
                typeof(bool),
                typeof(SearchResults),
                new PropertyMetadata(false)
            );

        public bool ShowSearchBar
        {
            get => (bool)GetValue(ShowSearchBarProperty);
            set => SetValue(ShowSearchBarProperty, value);
        }

        public SearchResults() {
            InitializeComponent();
        }
    }
}