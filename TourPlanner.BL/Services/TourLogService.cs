using System;
using System.Collections.Generic;
using TourPlanner.Models.Entities;
using TourPlanner.DAL.Repositories.TourLogRepository;

namespace TourPlanner.BL.Services;

public class TourLogService {
    private readonly ITourLogRepository _tourLogRepository;

    public TourLogService(
        ITourLogRepository tourLogRepository
    ) {
        _tourLogRepository = tourLogRepository;
    }
    
    public IEnumerable<TourLog> GetTourLogs(Tour tour) {
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

    public IEnumerable<TourLog> SearchTourLogs(string query) {
        return _tourLogRepository.SearchTourLogs(query);
    }
}