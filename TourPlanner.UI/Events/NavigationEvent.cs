using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.UI.Events
{
    public class NavigationEvent
    {
        public enum Destination
        {
            Home,
            CreateTour,
            CreateTourLog,
            LogViewer
        }
        public Destination Destin { get; }

        public NavigationEvent(Destination destination)
        {
            Destin = destination;
        }
    }
}
