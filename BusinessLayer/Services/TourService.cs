using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.BusinessLayer.Services
{
    public class TourService
    {
        //  TODO: unsure about the use of this class ... data base operations?
        public Tour CreateTour(Tour tour)
        {
            ValidateTourData(tour);

            return tour;
        }

        public void UpdateTour(Tour tour, string name, DateTime date, string description, string transportType, string from, string to)
        {
            //  TODO: fix this method
            //  Could try with a switch using "valueToChange" & "value" parameters later
            ValidateTourData(tour);

            tour.Name = name;
            tour.Date = date;
            tour.Description = description;
            tour.TransportType = transportType;
            tour.From = from;
            tour.To = to;
        }

        public void DeleteTour(Tour tour)
        {
            //  Database stuff, probably
        }

        public void ValidateTourData(Tour tour)
        {
            if (tour.Name.Length <= 0 || tour.Name.Length >= 120)
            {
                throw new ArgumentException("Invalid name length.");
            }
            //  If statement to check if date is less than the current day
            if (tour.Date == null)
            {
                throw new ArgumentException("Date cannot be empty.");
            }
            if (tour.Description.Length >= 255)
            {
                throw new ArgumentException("Description cannot be longer than 255 words.");
            }
            if (tour.TransportType != "Plane" || tour.TransportType != "Bus" || tour.TransportType != "Car" || tour.TransportType != "Train")
            {
                throw new ArgumentException("Invalid Transport Type.");
            }
            if (tour.Name.Length <= 0 || tour.Name.Length >= 255)
            {
                throw new ArgumentException("Invalid From location length.");
            }
            if (tour.Name.Length <= 0 || tour.Name.Length >= 255)
            {
                throw new ArgumentException("Invalid To location length.");
            }
        }
    }
}
