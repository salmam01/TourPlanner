using Microsoft.Extensions.Logging;
using Npgsql;
using TourPlanner.DAL.Exceptions;
using TourPlanner.DAL.Repositories.TourLogRepository;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;

namespace TourPlanner.UI.Services;

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
    
    public Result GetAllTourLogs(Tour tour) {
        try
        {
            return new Result(Result.ResultCode.Success, _tourLogRepository.GetTourLogs(tour.Id).ToList());
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while retrieving a list of all Tour Logs.Message: {Message}",
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while retrieving a list of all Tour Logs.");
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public IEnumerable<TourLog> SearchTourLogs(string query)
    {
        return _tourLogRepository.SearchTourLogs(query);
    }

    public Result CreateTourLog(Guid tourId, TourLog tourLog) {
        try
        {
            if (tourId == Guid.Empty)
            {
                _logger.LogWarning("Trying to create TourLog with empty Tour ID.");
                return new Result(Result.ResultCode.NullError);
            }
            if (tourLog == null)
            {
                _logger.LogWarning("Trying to create TourLog with NULL TourLog.");
                return new Result(Result.ResultCode.NullError);
            }

            tourLog.TourId = tourId;
            _tourLogRepository.InsertTourLog(tourLog);
            _logger.LogInformation("TourLog created => {@TourLog}", tourLog);
            return new Result(Result.ResultCode.Success);
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while creating TourLog => {TourLogID}. Message: {Message}",
                tourLog.Id,
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while creating TourLog => {TourLogID}", tourLog.Id);
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result UpdateTourLog(TourLog tourLog) {
        try
        {
            if (tourLog == null)
            {
                _logger.LogWarning("Trying to update TourLog with NULL TourLog.");
                return new Result(Result.ResultCode.NullError);
            }

            _tourLogRepository.UpdateTourLog(tourLog);
            _logger.LogInformation("TourLog updated => {@TourLog}", tourLog);
            return new Result(Result.ResultCode.Success);
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while updating TourLog => {TourLogID}. Message: {Message}", 
                tourLog.Id,
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while updating TourLog => {TourLogID}", tourLog.Id);
            return new Result(Result.ResultCode.UnknownError);
        }
    }

    public Result DeleteTourLog(TourLog tourLog) {
        try
        {
            if (tourLog == null)
            {
                _logger.LogWarning("Trying to delete TourLog with NULL TourLog.");
                return new Result(Result.ResultCode.NullError);
            }

            _tourLogRepository.DeleteTourLog(tourLog.Id);
            _logger.LogInformation("TourLog deleted => {@TourLogID}", tourLog.Id);
            return new Result(Result.ResultCode.Success);
        }
        catch (DatabaseException dbEx)
        {
            _logger.LogCritical(
                dbEx,
                "Database Exception occurred while deleting TourLog => {TourLogID}. Message: {Message}", 
                tourLog.Id,
                dbEx.Message
            );
            return new Result(Result.ResultCode.DatabaseError);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while deleting TourLog => {TourLogID}", tourLog.Id);
            return new Result(Result.ResultCode.UnknownError);
        }
    }
}