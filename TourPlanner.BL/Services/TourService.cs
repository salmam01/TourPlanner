using Microsoft.Extensions.Logging;
using Npgsql;
using System.Linq;
using TourPlanner.DAL.Exceptions;
using TourPlanner.DAL.Repositories.TourRepository;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;

namespace TourPlanner.UI.Services;

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

    public Result GetAllTours() {
        try
        {
            return new Result(Result.ResultCode.Success, _tourRepository.GetTours().ToList());
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while retrieving a list of all Tours. Message: {Message}",
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving a list of all Tours.");
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result SearchToursAndLogs(string query, IEnumerable<TourLog> tourLogsResult)
    {
        try
        {
            //  Query tours
            List<Tour> tours = _tourRepository.SearchTours(query).ToList();

            //  Extract tour Ids from the tour log list
            List<Guid> tourIds = [];
            foreach (TourLog log in tourLogsResult)
            {
                if (!tourIds.Contains(log.TourId))
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
            return new Result(Result.ResultCode.Success, tours.OrderByDescending(t => t.TourAttributes.SearchAlgorithmRanking).ToList());
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while searching Tours. Message:{Message}",
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while searching Tours.");
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result CreateTour(Tour tour) {
        try
        {
            if (tour == null)
            {
                _logger.LogWarning("Trying to create Tour with NULL Tour.");
                return new Result(Result.ResultCode.NullError);
            }

            _tourRepository.InsertTour(tour);

            Result result = RecalculateTourAttributes(tour);
            if (result.Code != Result.ResultCode.Success)
            {
                return result;
            }
            _logger.LogInformation("Tour created => {@Tour}", tour);
            return new Result(Result.ResultCode.Success);
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while creating Tour => {TourName}. Message: {Message}", 
                tour.Name,
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating Tour => {TourName}", tour.Name);
            return new Result(Result.ResultCode.UnknownError);
        }
    }
    
    public Result EditTour(Tour tour) {
        try
        {
            if (tour == null)
            {
                _logger.LogWarning("Trying to update Tour with NULL Tour.");
                return new Result(Result.ResultCode.NullError);
            }

            _tourRepository.UpdateTour(tour);

            Result result = RecalculateTourAttributes(tour);
            if (result.Code != Result.ResultCode.Success)
            {
                return result;
            }

            _logger.LogInformation("Tour updated => {Tour}", tour);
            return new Result(Result.ResultCode.Success);
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while updating Tour => {TourName}: {Message}", 
                tour.Name,
                dbEx.Message
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
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while recalculating Tour Attributes => {TourName}: {Message}",
                tour.Name,
                dbEx.Message
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
        try
        {
            if (tour == null)
            {
                _logger.LogWarning("Trying to delete Tour with NULL Tour.");
                return new Result(Result.ResultCode.NullError);
            }

            _tourRepository.DeleteTour(tour.Id);
            _logger.LogInformation("Tour deleted => {@TourName}", tour.Name);
            return new Result(Result.ResultCode.Success);
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while deleting Tour => {TourName}: {Message}", 
                tour.Name,
                dbEx.Message
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
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while deleting all Tours: {Message}",
                dbEx.Message
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