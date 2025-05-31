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
    private readonly TourAttributesService _tourAttributesService;

    public TourService(
        ITourRepository tourRepository,
        ITourLogRepository tourLogRepository,
        TourAttributesService tourAttributesService
    ) {
        _tourRepository = tourRepository;
        _tourLogRepository = tourLogRepository;
        _tourAttributesService = tourAttributesService;
    }

    public Tour GetTourById(Guid tourId) {
        Tour tour = _tourRepository.GetTourById(tourId);
        _tourAttributesService.UpdateAttributes(tour);
        return tour;
    }

    public IEnumerable<Tour> GetAllTours() {
        IEnumerable<Tour> tours = _tourRepository.GetTours();
        foreach (Tour tour in tours)
        {
            _tourAttributesService.UpdateAttributes(tour);
        }
        return tours;
    }
    
    public void CreateTour(Tour tour) {
        _tourRepository.InsertTour(tour);
        _tourAttributesService.InsertTourAttributes(tour.TourAttributes);
    }
    
    public void UpdateTour(Tour tour) {
        _tourRepository.UpdateTour(tour);
    }
    public IEnumerable<Tour> SearchTours(string query) {
        IEnumerable<Tour> tours = _tourRepository.SearchTours(query);
        return tours;
    }

    public void DeleteTour(Tour tour) {
        if (tour != null) {
            _tourRepository.DeleteTour(tour.Id);
        }
    }
}