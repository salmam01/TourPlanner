using System;
using System.Windows.Controls;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner.UILayer.Views
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
            DataContext = new HomeViewModel();
        }
    }
}
