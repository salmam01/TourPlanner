using TourPlanner.DAL.Data;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourAttributesRepository
{
    public class TourAttributesRepository : ITourAttributesRepository
    {
        private readonly TourPlannerDbContext _dbContext;
        public TourAttributesRepository(TourPlannerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<TourAttributes> GetTourAttributes(Guid tourId)
        {
            return _dbContext.TourAttributes.ToList();
        }

        public void UpdateTourAttributes(TourAttributes tourAttributes)
        {
            TourAttributes tourAttributesToUpdate = _dbContext.TourAttributes.Find(tourAttributes.Id);
            if (tourAttributesToUpdate == null) return;

            tourAttributesToUpdate.Popularity = tourAttributes.Popularity;
            tourAttributesToUpdate.ChildFriendliness = tourAttributes.ChildFriendliness;
            tourAttributesToUpdate.SearchAlgorithmRanking = tourAttributes.SearchAlgorithmRanking;

            Save();
        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }
    }
}
