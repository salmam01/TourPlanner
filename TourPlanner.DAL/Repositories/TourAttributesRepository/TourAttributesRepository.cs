using Microsoft.EntityFrameworkCore;
using Npgsql;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Exceptions;
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

        public void UpdateTourAttributes(TourAttributes tourAttributes)
        {
            TourAttributes? tourAttributesToUpdate = _dbContext.TourAttributes.Find(tourAttributes.Id);
            if (tourAttributesToUpdate == null) 
                throw new DatabaseException($"Error while retrieving Tour Attributes with ID {tourAttributes.Id}.");

            tourAttributesToUpdate.Popularity = tourAttributes.Popularity;
            tourAttributesToUpdate.ChildFriendliness = tourAttributes.ChildFriendliness;
            tourAttributesToUpdate.SearchAlgorithmRanking = tourAttributes.SearchAlgorithmRanking;

            Save();
        }

        private void Save()
        {
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                if (IsDatabaseException(ex))
                    throw new DatabaseException("Error while saving Tour Attribute Database changes.", ex);
                else
                    throw;
            }
        }

        private bool IsDatabaseException(Exception ex)
        {
            return (ex is DbUpdateException ||
                    ex is PostgresException ||
                    ex is InvalidOperationException);
        }
    }
}
