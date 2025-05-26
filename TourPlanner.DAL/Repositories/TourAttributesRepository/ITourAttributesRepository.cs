using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourAttributesRepository
{
    public interface ITourAttributesRepository
    {
        IEnumerable<TourAttributes> GetTourAttributes(Guid tourId);
        void InsertTourAttributes(TourAttributes tourAttributes);
        void UpdateTourAttributes(TourAttributes tourAttributes);
        void Save();
    }
}
