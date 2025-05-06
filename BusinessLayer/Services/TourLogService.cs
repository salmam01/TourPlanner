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

        public void GetAllTourLogs(Tour tour)
        {
            _tourLogRepository.GetTourLogs(tour.Id);
        }

        public void CreateTourLog(Tour tour, TourLog tourLog)
        {
            tour.TourLogs.Add(tourLog);
            tourLog.TourId = tour.Id;

            _tourLogRepository.InsertTourLog(tour.Id, tourLog);
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

        public void DeleteTourLog(TourLog tourLog, Tour tour) {
            tour.TourLogs?.Remove(tourLog);
            _tourLogRepository.DeleteTourLog(tourLog.Id);
        }
    }
}
