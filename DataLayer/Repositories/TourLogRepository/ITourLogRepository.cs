using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.DataLayer.Repositories.TourLogRepository
{
    public interface ITourLogRepository
    {
        TourLog GetTourLog(Guid tourLogId);
        IEnumerable<TourLog> GetTourLogs(Guid tourId);

        void InsertTourLog(Guid tourId, TourLog tourLog);
        void UpdateTourLog(TourLog tourLog);
        void DeleteTourLog(Guid tourLogId);
        void Save();
    }
}
