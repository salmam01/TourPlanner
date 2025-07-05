using System;
using System.Collections.Generic;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourLogRepository;

public interface ITourLogRepository {
    IEnumerable<TourLog> GetTourLogs(Guid tourId);
    IEnumerable<TourLog> SearchTourLogs(string query);
    void InsertTourLog(TourLog tourLog);
    void UpdateTourLog(TourLog tourLog);
    void DeleteTourLog(Guid tourLogId);
}