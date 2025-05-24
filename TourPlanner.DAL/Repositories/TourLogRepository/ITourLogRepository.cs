using System;
using System.Collections.Generic;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourLogRepository;

public interface ITourLogRepository {
    TourLog GetTourLog(Guid tourLogId);
    IEnumerable<TourLog> GetTourLogs(Guid tourId);
    IEnumerable<TourLog> SearchTourLogs(string query);

    void InsertTourLog(Guid tourId, TourLog tourLog);
    void UpdateTourLog(TourLog tourLog);
    void DeleteTourLog(Guid tourLogId);
    void Save();
}