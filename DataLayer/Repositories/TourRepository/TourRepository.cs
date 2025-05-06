using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Data;

namespace TourPlanner.DataLayer.Repositories.TourRepository
{
    public class TourRepository : ITourRepository
    {
        private TourPlannerDbContext _context;

        public TourRepository(TourPlannerDbContext context)
        {
            _context = context;
        }

        public Tour GetTourById(Guid tourId) => _context.Tours.Find(tourId);
        
        public IEnumerable<Tour> GetTours() => _context.Tours;

        public void InsertTour(Tour tour)
        {
            _context.Tours.Add(tour);
            Save();
        }

        public void UpdateTour(Tour tour)
        {
            //  TODO: Implement
            Save();
        }

        public void DeleteTour(Guid tourId)
        {
            Tour tour = _context.Tours.Find(tourId);
            _context.Tours.Remove(tour);
            Save();
        }

        public void Save()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("DB Update Failed: " + ex.InnerException?.Message);
                throw;
            }
        }
    }
}
