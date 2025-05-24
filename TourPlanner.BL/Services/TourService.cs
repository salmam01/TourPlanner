using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.Models.Entities;
using TourPlanner.DAL.Repositories.TourLogRepository;
using TourPlanner.DAL.Repositories.TourRepository;

namespace TourPlanner.BL.Services;

public class TourService {
    private readonly ITourLogRepository _tourLogRepository;
    private readonly ITourRepository _tourRepository;
    
    public TourService(ITourRepository tourRepository, ITourLogRepository tourLogRepository) {
        _tourRepository = tourRepository;
        _tourLogRepository = tourLogRepository;
    }
    public Tour GetTourById(Guid tourId) {
        var tour = _tourRepository.GetTourById(tourId);
        TourAttributesService.UpdateAttributes(tour);
        return tour;
    }
    public IEnumerable<Tour> GetAllTours() {
        var tours = _tourRepository.GetTours();
        foreach (var tour in tours)
        {
            TourAttributesService.UpdateAttributes(tour);
        }
        return tours;
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
    
    public (IEnumerable<Tour> Tours, IEnumerable<TourLog> Logs) SearchToursAndLogs(string query, double? minPopularity, bool? childFriendliness)
    {
        IEnumerable<Tour> tours = _tourRepository.SearchTours(query, minPopularity, childFriendliness);
        // Filtering on logs by popularity/child-friendliness doesn't make sense. Return empty logs for this path.
        IEnumerable<TourLog> logs = Enumerable.Empty<TourLog>();
        return (tours, logs);
    }
    
    public (IEnumerable<Tour> Tours, IEnumerable<TourLog> Logs) SearchToursAndLogs(string query) {
        IEnumerable<Tour> tours = _tourRepository.SearchTours(query);
        IEnumerable<TourLog> logs = _tourLogRepository.SearchTourLogs(query);
        return (tours, logs);
    }
}