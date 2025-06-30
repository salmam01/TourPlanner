using Microsoft.Extensions.Logging;
using Npgsql;
using System.Linq;
using TourPlanner.BL.Utils;
using TourPlanner.DAL.Repositories.TourRepository;
using TourPlanner.Models.Entities;

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

    public List<Tour> SearchToursAndLogs(string query, IEnumerable<TourLog> tourLogsResult)
    {
        //  Query tours
        List<Tour> tours = _tourRepository.SearchTours(query).ToList();

        //  Extract tour Ids from the tour log list
        List<Guid> tourIds = [];
        foreach (TourLog log in tourLogsResult)
        {
            if(!tourIds.Contains(log.TourId))
            {
                tourIds.Add(log.TourId);
            }
        }

        //  Get the tours from the database & combine the list
        foreach (Guid tourId in tourIds)
        {
            Tour? tour = _tourRepository.GetTourById(tourId);
            if (tour != null && !tours.Contains(tour))
            {
                tours.Add(tour);
            }
        }

        //  Sort the list by algorithm & display combined list
        return tours.OrderByDescending(t => t.TourAttributes.SearchAlgorithmRanking).ToList();
    }

    public Result CreateTour(Tour tour) {
        if (tour == null)
        {
            _logger.LogWarning("Trying to create Tour with NULL Tour.");
            return new Result(Result.ResultCode.NullError);
        }

        try
        {
            _tourRepository.InsertTour(tour);
            _logger.LogInformation("Tour created => {@Tour}", tour);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx, 
                "Postgres Exception occurred while creating Tour => {TourName}: {ErrorCode} -> {Message}", 
                tour.Name, 
                pgEx.SqlState, 
                pgEx.MessageText
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating Tour => {TourName}", tour.Name);
            return new Result(Result.ResultCode.UnknownError);
        }
    }
    
    public Result UpdateTour(Tour tour) {
        if (tour == null)
        {
            _logger.LogWarning("Trying to update Tour with NULL Tour.");
            return new Result(Result.ResultCode.NullError);
        }

        try
        {
            _tourRepository.UpdateTour(tour);
            _logger.LogInformation("Tour updated => {Tour}", tour);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx, 
                "Postgres Exception occurred while updating Tour => {TourName}: {ErrorCode} -> {Message}", 
                tour.Name, 
                pgEx.SqlState, 
                pgEx.MessageText
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating Tour => {TourName}", tour.Name);
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result RecalculateTourAttributes(Tour tour)
    {
        if (tour == null)
        {
            _logger.LogWarning("Trying to recalculate Tour Attributes with NULL Tour.");
            return new Result(Result.ResultCode.NullError);
        }
        
        try
        {
            _tourAttributesService.UpdateAttributes(tour);
            _logger.LogInformation("Tour Attributes recalculated => {TourName}: {@TourAttributes}", tour.Name, tour.TourAttributes);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx, 
                "Postgres Exception occurred while recalculating Tour Attributes => {TourName}: {ErrorCode} -> {Message}", 
                tour.Name, 
                pgEx.SqlState, 
                pgEx.MessageText
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while recalculating Tour Attributes => {TourName}", tour.Name);
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result DeleteTour(Tour tour) 
    {
        if (tour == null) 
        {
            _logger.LogWarning("Trying to delete Tour with NULL Tour.");
            return new Result(Result.ResultCode.NullError);
        }

        try
        {
            _tourRepository.DeleteTour(tour.Id);
            _logger.LogInformation("Tour deleted => {@TourName}", tour.Name);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx, 
                "Postgres Exception occurred while deleting Tour => {TourName}: {ErrorCode} -> {Message}", 
                tour.Name, 
                pgEx.SqlState, 
                pgEx.MessageText
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting Tour => {TourName}", tour.Name);
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result DeleteAllTours()
    {
        try
        {
            _tourRepository.DeleteAllTours();
            _logger.LogInformation("All tours have been deleted");
            return new Result(Result.ResultCode.Success);
        } 
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx,
                "Postgres Exception occurred while deleting all Tours: {ErrorCode} -> {Message}",
                pgEx.SqlState,
                pgEx.MessageText
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting all Tours");
            return new Result(Result.ResultCode.UnknownError);
        }
    }
}