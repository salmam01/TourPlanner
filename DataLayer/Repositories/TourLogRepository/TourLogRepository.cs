using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Data;

namespace TourPlanner.DataLayer.Repositories.TourLogRepository
{
    public class TourLogRepository : ITourLogRepository
    {
        private TourPlannerDbContext _context;
        public TourLogRepository(TourPlannerDbContext context)
        {
            _context = context;
        }

        public TourLog GetTourLog(Guid TourId) => _context.TourLogs.Find(TourId);

        public IEnumerable<TourLog> GetTourLogs(Guid tourId)
        {
            return _context.TourLogs
                .Where(log => log.TourId == tourId)
                .ToList();
        }

        public IEnumerable<TourLog> GetTourLogs() => _context.TourLogs.ToList();

        public void InsertTourLog(Guid tourId, TourLog tourLog)
        {
            _context.TourLogs.Add(tourLog);
            Save();
        }

        public void UpdateTourLog(TourLog tourLog)
        {
            TourLog tourLogToUpdate = _context.TourLogs.Find(tourLog.Id);
            if (tourLogToUpdate == null) return;

            tourLogToUpdate.Rating = tourLog.Rating;
            tourLogToUpdate.Difficulty = tourLog.Difficulty;
            tourLogToUpdate.TotalDistance = tourLog.TotalDistance;
            tourLogToUpdate.TotalTime = tourLog.TotalTime;
            tourLogToUpdate.Comment = tourLog.Comment;
            tourLogToUpdate.Date = tourLog.Date;

            Save();
        }

        public void DeleteTourLog(Guid tourLogId)
        {
            TourLog tourLog = _context.TourLogs.Find(tourLogId);
            _context.TourLogs.Remove(tourLog);
            Save();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

    }
}
