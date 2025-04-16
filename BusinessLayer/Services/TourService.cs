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

        public Tour CreateTour(string name, DateTime date, string description, char transportType, string from, string to)
        {
            ValidateTourData(name, date, description, transportType, from, to);

            Tour tour = new Tour(
                name,
                date,
                description,
                transportType,
                from,
                to
            );

            return tour;
        }

        public void UpdateTour(Tour tour, string name, DateTime date, string description, char transportType, string from, string to)
        {
            //  Could try with a switch using "valueToChange" & "value" parameters later
            ValidateTourData(name, date, description, transportType, from, to);

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

        public void ValidateTourData(string name, DateTime date, string description, char transportType, string from, string to)
        {
            if (name.Length <= 0 || name.Length >= 120)
            {
                throw new ArgumentException("Invalid name length.");
            }
            //  If statement to check if date is less than the current day
            if (date == null)
            {
                throw new ArgumentException("Date cannot be empty.");
            }
            if (description.Length >= 255)
            {
                throw new ArgumentException("Description cannot be longer than 255 words.");
            }
            if (transportType != 'C' || transportType != 'P' || transportType != 'T')
            {
                throw new ArgumentException("Invalid Transport Type.");
            }
            if (from.Length <= 0 || from.Length >= 255)
            {
                throw new ArgumentException("Invalid From location length.");
            }
            if (to.Length <= 0 || to.Length >= 255)
            {
                throw new ArgumentException("Invalid To location length.");
            }
        }
    }
}
