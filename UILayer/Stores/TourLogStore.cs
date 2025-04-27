using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.UILayer.Stores
{
    public class TourLogStore
    {
        public event Action<TourLog> TourLogCreated;

        public void CreateTourLog(TourLog tourLog)
        {
            //  Invokes an event if anyone is subscribed
            TourLogCreated?.Invoke(tourLog);
        }
    }
}
