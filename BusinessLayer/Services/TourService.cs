using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Repositories.TourRepository;

namespace TourPlanner.BusinessLayer.Services
{
    public class TourService
    {
        private readonly ITourRepository _tourRepository;
        public TourService(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }

        public Tour GetTourById(Guid tourId)
        {
            return _tourRepository.GetTourById(tourId);
        }

        public IEnumerable<Tour> GetAllTours()
        {
            return _tourRepository.GetTours();
        }

        public void CreateTour(Tour tour)
        {
            //  TODO: Make sure tour doesn't already exist
            _tourRepository.InsertTour(tour);
        }

        public void UpdateTour(Tour tour)
        {
            _tourRepository.UpdateTour(tour);
        }

        public void DeleteTour(Tour tour)
        {
            if(tour != null)
            {
                _tourRepository.DeleteTour(tour.Id);
            }
        }
    }
}
