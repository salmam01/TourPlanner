using System;
using System.Windows.Controls;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner.UILayer.Views
{
    /// <summary>
    /// Interaction logic for TourList.xaml
    /// </summary>
    public partial class TourList : UserControl
    {
        public TourList()
        {
            InitializeComponent();
            Loaded += (s, e) => {
                Console.WriteLine($"TourList DataContext: {DataContext?.GetType().Name}");
                if (DataContext is TourListViewModel vm)
                {
                    Console.WriteLine($"Tours count: {vm.Tours?.Count}");
                }
            };
        }
    }
}
