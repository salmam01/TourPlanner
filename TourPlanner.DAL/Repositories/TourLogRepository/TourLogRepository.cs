using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Exceptions;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourLogRepository;

public class TourLogRepository : ITourLogRepository {
    private readonly TourPlannerDbContext _dbContext;

    public TourLogRepository(TourPlannerDbContext context) 
    {
        _dbContext = context;
    }

    public IEnumerable<TourLog> GetTourLogs(Guid tourId) 
    {
        try
        {
            return _dbContext.TourLogs
                .Where(log => log.TourId == tourId)
                .ToList();
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException($"Error while retrieving Tours Logs with Tour ID {tourId}.", ex);
            else
                throw;
        }
    }

    public IEnumerable<TourLog> SearchTourLogs(string query)
    {
        try
        {
            string ftsQuery = query.Trim().Replace(" ", " & ") + ":*";
            return _dbContext.TourLogs.FromSqlRaw(
                "SELECT * FROM \"TourLogs\" " +
                "WHERE \"SearchVector\" @@ to_tsquery('simple', {0})",
                ftsQuery
            ).ToList();
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while performing Full-Text Search on Tour Logs.", ex);
            else
                throw;
        }
    }

    public void InsertTourLog(TourLog tourLog) 
    {
        _dbContext.TourLogs.Add(tourLog);
        Save();
    }
    
    public void UpdateTourLog(TourLog tourLog) 
    {
        TourLog? tourLogToUpdate = _dbContext.TourLogs.Find(tourLog.Id);
        if (tourLogToUpdate == null)
            throw new DatabaseException($"Could not find Tour Log with ID {tourLog.Id} to update.");

        tourLogToUpdate.Rating = tourLog.Rating;
        tourLogToUpdate.Difficulty = tourLog.Difficulty;
        tourLogToUpdate.TotalDistance = tourLog.TotalDistance;
        tourLogToUpdate.TotalTime = tourLog.TotalTime;
        tourLogToUpdate.Comment = tourLog.Comment;
        tourLogToUpdate.Date = tourLog.Date;
        
        Save();
    }
    
    public void DeleteTourLog(Guid tourLogId) 
    {
        TourLog? tourLog = _dbContext.TourLogs.Find(tourLogId);
        if (tourLog == null)
            throw new DatabaseException($"Could not find Tour Log with ID {tourLogId} to delete.");

        _dbContext.TourLogs.Remove(tourLog);
        Save();
    }
    
    private void Save() 
    {
        try
        {
            _dbContext.SaveChanges();
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while saving Tour Log Database changes.", ex);
            else
                throw;
        }
    }

    private bool IsDatabaseException(Exception ex)
    {
        return (ex is DbUpdateException ||
                ex is PostgresException ||
                ex is InvalidOperationException);
    }
}