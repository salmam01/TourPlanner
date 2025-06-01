using Microsoft.Extensions.Logging;
using TourPlanner.DAL.Repositories.TourLogRepository;
using TourPlanner.Models.Entities;

namespace TourPlanner.BL.Services;

public class TourLogService {

    private readonly ITourLogRepository _tourLogRepository;
    private readonly ILogger<TourLogRepository> _logger;

    public TourLogService(
        ITourLogRepository tourLogRepository,
        ILogger<TourLogRepository> logger
    ) {
        _tourLogRepository = tourLogRepository;
        _logger = logger;
    }
    
    public IEnumerable<TourLog> GetAllTourLogs(Tour tour) {
        return _tourLogRepository.GetTourLogs(tour.Id);
    }

    public void CreateTourLog(Guid tourId, TourLog tourLog) {
        tourLog.TourId = tourId;
        _tourLogRepository.InsertTourLog(tourLog);
    }

    public void UpdateTourLog(TourLog tourLog) {
        _tourLogRepository.UpdateTourLog(tourLog);
    }

    public void DeleteTourLog(TourLog tourLog) {
        _tourLogRepository.DeleteTourLog(tourLog.Id);
    }

    public IEnumerable<TourLog> SearchTourLogs(Tour tour, string query) {
        return _tourLogRepository.SearchTourLogs(tour.Id, query);
    }
}