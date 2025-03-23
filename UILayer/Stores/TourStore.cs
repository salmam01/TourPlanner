using System;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.UILayer.Stores
{
    public class TourStore
    {
        public event Action<Tour> TourCreated;

        public void CreateTour(Tour tour)
        {
            //  Invokes an event if anyone is subscribed
            TourCreated?.Invoke(tour);
        }
    }
}
