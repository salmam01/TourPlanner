using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Repositories.TourLogRepository;

namespace TourPlanner.BusinessLayer.Services
{
    public class TourLogService
    {
        private readonly ITourLogRepository _tourLogRepository;
        public TourLogService(ITourLogRepository tourLogRepository)
        {
            _tourLogRepository = tourLogRepository;
        }

        public IEnumerable<TourLog> GetTourLogs(Tour tour)
        {
            return _tourLogRepository.GetTourLogs(tour.Id);
        }

        public void CreateTourLog(Guid tourId, TourLog tourLog)
        {
            tourLog.TourId = tourId;
            _tourLogRepository.InsertTourLog(tourId, tourLog);
        }

        public void UpdateTourLog(TourLog tourLog, DateTime date, string comment, int difficulty, double rating, double totalDistance, TimeSpan totalTime)
        {
            tourLog.Date = date;
            tourLog.Comment = comment;
            tourLog.Difficulty = difficulty;
            tourLog.Rating = rating;
            tourLog.TotalDistance = totalDistance;
            tourLog.TotalTime = totalTime;

            _tourLogRepository.UpdateTourLog(tourLog);
        }

        public void DeleteTourLog(TourLog tourLog) {
            _tourLogRepository.DeleteTourLog(tourLog.Id);
        }
    }
}
