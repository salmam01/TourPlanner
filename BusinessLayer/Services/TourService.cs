using System;
using System.Collections.Generic;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Repositories.TourLogRepository;
using TourPlanner.DataLayer.Repositories.TourRepository;

namespace TourPlanner.BusinessLayer.Services;

public class TourService {
    private readonly ITourLogRepository _tourLogRepository;
    private readonly ITourRepository _tourRepository;
    
    public TourService(ITourRepository tourRepository, ITourLogRepository tourLogRepository) {
        _tourRepository = tourRepository;
        _tourLogRepository = tourLogRepository;
    }
    public Tour GetTourById(Guid tourId) {
        return _tourRepository.GetTourById(tourId);
    }
    public IEnumerable<Tour> GetAllTours() {
        return _tourRepository.GetTours();
    }
    
    public void CreateTour(Tour tour) {
        _tourRepository.InsertTour(tour);
    }
    
    public void UpdateTour(Tour tour) {
        _tourRepository.UpdateTour(tour);
    }
    
    public void DeleteTour(Tour tour) {
        if (tour != null) {
            _tourRepository.DeleteTour(tour.Id);
        }
    }
    
    public (IEnumerable<Tour> Tours, IEnumerable<TourLog> Logs) SearchToursAndLogs(string query) {
        IEnumerable<Tour> tours = _tourRepository.SearchTours(query);
        IEnumerable<TourLog> logs = _tourLogRepository.SearchTourLogs(query);
        return (tours, logs);
    }
}