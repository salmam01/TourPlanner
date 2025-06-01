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
