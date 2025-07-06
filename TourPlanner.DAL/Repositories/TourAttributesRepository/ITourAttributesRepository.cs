using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourAttributesRepository
{
    public interface ITourAttributesRepository
    {
        void UpdateTourAttributes(TourAttributes tourAttributes);
    }
}
