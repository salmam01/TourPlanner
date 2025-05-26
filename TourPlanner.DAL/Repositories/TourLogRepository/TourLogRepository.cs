using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.DAL.Data;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourLogRepository;

public class TourLogRepository : ITourLogRepository {
    private readonly TourPlannerDbContext _dbContext;

    public TourLogRepository(TourPlannerDbContext context) {
        _dbContext = context;
    }

    public IEnumerable<TourLog> SearchTourLogs(string query) {
        if (string.IsNullOrWhiteSpace(query))
        {
            return _dbContext.TourLogs.ToList();
        }

        string ftsQuery = query.Trim().Replace(" ", " & ") + ":*";
        var likeQuery = $"%{query.Trim()}%";
        
        // Suche auf mehreren Feldern + computed field
        return _dbContext.TourLogs.FromSqlRaw(
            "SELECT * FROM \"TourLogs\" " +
            "WHERE \"SearchVector\" @@ to_tsquery('english', {0}) " +
            "OR CAST(\"Difficulty\" AS TEXT) ILIKE {1} " +
            "OR CAST(\"Rating\" AS TEXT) ILIKE {1} " +
            "OR \"Comment\" ILIKE {1} " +
            "OR CONCAT(\"Comment\", ' ', \"Difficulty\", ' ', \"Rating\") ILIKE {1} ",
            ftsQuery,
            likeQuery
        ).ToList();
    }

    public TourLog GetTourLog(Guid TourId) {
        return _dbContext.TourLogs.Find(TourId);
    }

    public IEnumerable<TourLog> GetTourLogs(Guid tourId) {
        return _dbContext.TourLogs
            .Where(log => log.TourId == tourId)
            .ToList();
    }
    
    public void InsertTourLog(TourLog tourLog) {
        _dbContext.TourLogs.Add(tourLog);
        Save();
    }
    
    public void UpdateTourLog(TourLog tourLog) {
        TourLog tourLogToUpdate = _dbContext.TourLogs.Find(tourLog.Id);
        if (tourLogToUpdate == null) return;
        
        tourLogToUpdate.Rating = tourLog.Rating;
        tourLogToUpdate.Difficulty = tourLog.Difficulty;
        tourLogToUpdate.TotalDistance = tourLog.TotalDistance;
        tourLogToUpdate.TotalTime = tourLog.TotalTime;
        tourLogToUpdate.Comment = tourLog.Comment;
        tourLogToUpdate.Date = tourLog.Date;
        
        Save();
    }
    
    public void DeleteTourLog(Guid tourLogId) {
        TourLog tourLog = _dbContext.TourLogs.Find(tourLogId);
        _dbContext.TourLogs.Remove(tourLog);
        Save();
    }
    
    public void Save() {
        _dbContext.SaveChanges();
    }
}