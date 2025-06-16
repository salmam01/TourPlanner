using Microsoft.Extensions.Logging;
using Npgsql;
using TourPlanner.BL.Utils;
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

    public IEnumerable<TourLog> SearchTourLogs(Tour tour, string query)
    {
        return _tourLogRepository.SearchTourLogs(tour.Id, query);
    }

    public Result CreateTourLog(Guid tourId, TourLog tourLog) {
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

        try
        {
            tourLog.TourId = tourId;
            _tourLogRepository.InsertTourLog(tourLog);
            _logger.LogInformation("TourLog created => {@TourLog}", tourLog);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx, 
                "Postgres Exception occurred while creating TourLog => {TourLogID}: {ErrorCode} -> {Message}", 
                tourLog.Id, 
                pgEx.SqlState, 
                pgEx.MessageText
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
        if (tourLog == null)
        {
            _logger.LogWarning("Trying to update TourLog with NULL TourLog.");
            return new Result(Result.ResultCode.NullError);
        }

        try
        {
            _tourLogRepository.UpdateTourLog(tourLog);
            _logger.LogInformation("TourLog updated => {@TourLog}", tourLog);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx, 
                "Postgres Exception occurred while updating TourLog => {TourLogID}: {ErrorCode} -> {Message}", 
                tourLog.Id, 
                pgEx.SqlState, 
                pgEx.MessageText
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
        if(tourLog == null)
        {
            _logger.LogWarning("Trying to delete TourLog with NULL TourLog.");
            return new Result(Result.ResultCode.NullError);
        }

        try
        {
            _tourLogRepository.DeleteTourLog(tourLog.Id);
            _logger.LogInformation("TourLog deleted => {@TourLogID}", tourLog.Id);
            return new Result(Result.ResultCode.Success);
        }
        catch (PostgresException pgEx)
        {
            _logger.LogError(
                pgEx,
                "Postgres Exception occurred while deleting TourLog => {TourLogID}: {ErrorCode} -> {Message}", 
                tourLog.Id,
                pgEx.SqlState,
                pgEx.MessageText
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