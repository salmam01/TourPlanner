using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.DataLayer.Repositories
{
    public interface ITourLogRepository
    {
        TourLog GetTourLogById(Guid tourLogId);
        IEnumerable<TourLog> GetTourLogs();

        void InsertTourLog(TourLog tourLog);
        void UpdateTourLog(TourLog tourLog);
        void DeleteTourLog(Guid tourLogId);
        void Save();
    }
}
