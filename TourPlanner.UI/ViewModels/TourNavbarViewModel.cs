using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.UI.ViewModels
{
    public class TourNavbarViewModel : BaseViewModel
    {
        public MapViewModel MapViewModel { get; }

        public TourNavbarViewModel(MapViewModel mapViewModel)
        {
            MapViewModel = mapViewModel;
        }
    }
}
