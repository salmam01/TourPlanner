using TourPlanner.Models.Entities;
using TourPlanner.DAL.Repositories.TourRepository;
using Microsoft.Extensions.Logging;
using Npgsql;

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

    public IEnumerable<Tour> SearchTours(string query)
    {
        return _tourRepository.SearchTours(query);
    }

    public void CreateTour(Tour tour) {
        if (tour == null)
        {
            _logger.LogWarning("Trying to create Tour with NULL Tour.");
            return;
        }

        try
        {
            _tourRepository.InsertTour(tour);
            _logger.LogInformation("Tour created => {@Tour}", tour);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, "Postgres Exception occurred while creating Tour => {TourName}: {ErrorCode} -> {Message}", tour.Name, pgEx.SqlState, pgEx.MessageText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating Tour => {TourName}", tour.Name);
        }
    }
    
    public void UpdateTour(Tour tour) {
        if (tour == null)
        {
            _logger.LogWarning("Trying to update Tour with NULL Tour.");
            return;
        }

        try
        {
            _tourRepository.UpdateTour(tour);
            _logger.LogInformation("Tour updated => {Tour}", tour);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, "Postgres Exception occurred while updating Tour => {TourName}: {ErrorCode} -> {Message}", tour.Name, pgEx.SqlState, pgEx.MessageText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating Tour => {TourName}", tour.Name);
        }
    }

    public void RecalculateTourAttributes(Tour tour)
    {
        if (tour == null)
        {
            _logger.LogWarning("Trying to recalculate Tour Attributes with NULL Tour.");
            return;
        }
        
        try
        {
            _tourAttributesService.UpdateAttributes(tour);
            _logger.LogInformation("Tour Attributes recalculated => {TourName}: {@TourAttributes}", tour.Name, tour.TourAttributes);

        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, "Postgres Exception occurred while recalculating Tour Attributes => {TourName}: {ErrorCode} -> {Message}", tour.Name, pgEx.SqlState, pgEx.MessageText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while recalculating Tour Attributes => {TourName}", tour.Name);
        }
    }

    public void DeleteTour(Tour tour) {
        if (tour == null) 
        {
            _logger.LogWarning("Trying to delete Tour with NULL Tour.");
            return;
        }

        try
        {
            _tourRepository.DeleteTour(tour.Id);
            _logger.LogInformation("Tour deleted => {@TourName}", tour.Name);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(pgEx, "Postgres Exception occurred while deleting Tour => {TourName}: {ErrorCode} -> {Message}", tour.Name, pgEx.SqlState, pgEx.MessageText);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting Tour => {TourName}", tour.Name);
        }
    }
}