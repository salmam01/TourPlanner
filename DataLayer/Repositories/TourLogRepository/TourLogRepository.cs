using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourPlanner.BusinessLayer.Models;
using TourPlanner.DataLayer.Data;

namespace TourPlanner.DataLayer.Repositories.TourLogRepository;

public class TourLogRepository : ITourLogRepository {
    private readonly TourPlannerDbContext _context;

    public TourLogRepository(TourPlannerDbContext context) {
        _context = context;
    }

    public IEnumerable<TourLog> SearchTourLogs(string query) {
        if (string.IsNullOrWhiteSpace(query))
            return _context.TourLogs.ToList();

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