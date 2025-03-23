using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TourPlanner.UILayer.ViewModels;

namespace TourPlanner.UILayer.Views
{
    /// <summary>
    /// Interaction logic for CreateTourLog.xaml
    /// </summary>
    public partial class CreateTourLog : UserControl // for me:   UserControl ist eine Klasse, die als Basisklasse für alle Steuerelemente in WPF verwendet wird
    {
        public CreateTourLog()
        {
            InitializeComponent();
            DataContext = new CreateTourLogViewModel(); // for me:  Verknüpft das ViewModel mit der View und ermöglicht so die Datenbindung zwischen UI-Elementen und den Eigenschaften im ViewModel

        } 
    }
}
