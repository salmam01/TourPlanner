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

        public void UpdateTourLog(TourLog tourLog)
        {
            _tourLogRepository.UpdateTourLog(tourLog);
        }

        public void DeleteTourLog(TourLog tourLog) {
            _tourLogRepository.DeleteTourLog(tourLog.Id);
        }
    }
}
