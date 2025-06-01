using TourPlanner.Models.Entities;
using TourPlanner.DAL.Repositories.TourRepository;
using Microsoft.Extensions.Logging;

namespace TourPlanner.BL.Services;

public class TourService {
    private readonly ITourRepository _tourRepository;
    private readonly TourAttributesService _tourAttributesService;
    private readonly ILogger<TourService> _logger;

    public TourService(
        ITourRepository tourRepository,
        TourAttributesService tourAttributesService,
        ILogger<TourService> logger
    ) {
        _tourRepository = tourRepository;
        _tourAttributesService = tourAttributesService;
        _logger = logger;
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

    public IEnumerable<Tour> SearchTours(string query) {
        return _tourRepository.SearchTours(query);
    }

    public void RecalculateTourAttributes(Tour tour)
    {
        _tourAttributesService.UpdateAttributes(tour);
    }

    public void DeleteTour(Tour tour) {
        if (tour != null) {
            _tourRepository.DeleteTour(tour.Id);
        }
    }
}