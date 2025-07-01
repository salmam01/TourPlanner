using System;
using System.Windows.Controls;


namespace TourPlanner.UI.Views
{
    /// <summary>
    /// Interaction logic for TourManagement.xaml
    /// </summary>
    public partial class TourManagement : UserControl
    {
        public TourManagement()
        {
            InitializeComponent();
        }

        public void MoreActionsButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.Button;
            if (button?.ContextMenu != null)
            {
                button.ContextMenu.PlacementTarget = button;
                button.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                button.ContextMenu.IsOpen = true;
            }
        }
    }
}
