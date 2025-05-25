using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.DAL.Data;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourLogRepository;

public class TourLogRepository : ITourLogRepository {
    private readonly ILogger<TourLogRepository> _logger;

    private readonly TourPlannerDbContext _context;

    public TourLogRepository(TourPlannerDbContext context, ILogger<TourLogRepository> logger) {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<TourLog> SearchTourLogs(string query) {
        if (string.IsNullOrWhiteSpace(query))
        {
            _logger.LogWarning("Query is empty.");
            return _context.TourLogs.ToList();
        }

        string ftsQuery = query.Trim().Replace(" ", " & ") + ":*";
        var likeQuery = $"%{query.Trim()}%";
        
        // Suche auf mehreren Feldern + computed field
        return _context.TourLogs.FromSqlRaw(
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
        return _context.TourLogs.Find(TourId);
    }

    public IEnumerable<TourLog> GetTourLogs(Guid tourId) {
        return _context.TourLogs
            .Where(log => log.TourId == tourId)
            .ToList();
    }
    
    public void InsertTourLog(Guid tourId, TourLog tourLog) {
        _context.TourLogs.Add(tourLog);
        Save();
    }
    
    public void UpdateTourLog(TourLog tourLog) {
        TourLog tourLogToUpdate = _context.TourLogs.Find(tourLog.Id);
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
        TourLog tourLog = _context.TourLogs.Find(tourLogId);
        _context.TourLogs.Remove(tourLog);
        Save();
    }
    
    public void Save() {
        _context.SaveChanges();
    }
    
    public IEnumerable<TourLog> GetTourLogs() {
        return _context.TourLogs.ToList();
    }
}