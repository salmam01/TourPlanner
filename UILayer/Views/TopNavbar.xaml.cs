using System.Windows;
using System.Windows.Controls;

namespace TourPlanner.UILayer.Views
{
    public partial class TopNavbar : UserControl
    {
        public TopNavbar()
        {
            InitializeComponent();
            // SearchBar logic removed as the SearchBar has been deleted from XAML.
        }
        // ButtonToggleSearch_Click removed because SearchBar is no longer present.
    }
}
