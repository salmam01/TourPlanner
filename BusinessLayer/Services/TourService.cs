using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Repositories;

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

        public void UpdateTour(Tour tour, string name, DateTime date, string description, string transportType, string from, string to)
        {
            //  TODO: fix this method
            //  Could try with a switch using "valueToChange" & "value" parameters later
            //ValidateTourData(tour);

            tour.Name = name;
            tour.Date = date;
            tour.Description = description;
            tour.TransportType = transportType;
            tour.From = from;
            tour.To = to;
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
