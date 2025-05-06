using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.DataLayer.Repositories.TourRepository
{
    public interface ITourRepository
    {
        Tour GetTourById(Guid tourId);
        IEnumerable<Tour> GetTours();

        void InsertTour(Tour tour);
        void UpdateTour(Tour tour);
        void DeleteTour(Guid tourId);
        void Save();
    }
}
