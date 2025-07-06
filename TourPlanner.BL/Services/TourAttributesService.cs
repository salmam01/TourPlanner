using Microsoft.Extensions.Logging;
using Npgsql;
using TourPlanner.DAL.Repositories.TourAttributesRepository;
using TourPlanner.Models.Entities;

namespace TourPlanner.BL.Services
{
    public class TourAttributesService
    {
        private readonly ITourAttributesRepository _tourAttributesRepository;
        private readonly ILogger<TourAttributesService> _logger;

        public TourAttributesService(
            ITourAttributesRepository tourAttributesRepository,
            ILogger<TourAttributesService> logger
        ){
            _tourAttributesRepository = tourAttributesRepository;
            _logger = logger;
        }

        /// <summary>
        /// Computes popularity based on the number of logs.
        /// </summary>
        public int ComputePopularity(ICollection<TourLog> logs)
        {
            return logs?.Count ?? 0;
        }

        /// <summary>
        /// Computes child-friendliness based on average difficulty, time, and distance.
        /// Returns true if child-friendly, false otherwise.
        /// </summary>
        public bool ComputeChildFriendliness(Tour tour)
        {
            if (tour.TourLogs == null || tour.TourLogs.Count == 0)
            {
                return (tour.Distance / 1000) <= 10.0 &&
                       tour.EstimatedTime.TotalMinutes <= 150.0;
            }

            // Average difficulty: 1-5, threshold <= 2 (easy/moderate)
            double avgDifficulty = tour.TourLogs.Average(l => l.Difficulty);
            // Average distance: threshold < 10km (adjust as needed)
            double avgDistance = tour.TourLogs.Average(l => l.TotalDistance / 1000);
            // Average time: threshold < 2.5h (in minutes)
            double avgMinutes = tour.TourLogs.Average(l => l.TotalTime.TotalMinutes);

            return
                avgDifficulty <= 2.0 &&
                avgDistance <= 10.0 &&
                avgMinutes <= 150.0;
        }

        public double ComputeSearchAlgorithmRanking(double Popularity, bool Childfriendliness)
        {
            double SearchAlgorithmRanking = Popularity / 100;

            if (Childfriendliness)
                SearchAlgorithmRanking *= 1.5;

            return Math.Min(SearchAlgorithmRanking, 1.0);
        }

        /// <summary>
        /// Updates a Tour's TourAttributes object based on logs.
        /// </summary>
        public void UpdateAttributes(Tour tour)
        {
            //  Errors are handled by TourService
            tour.TourAttributes.Popularity = ComputePopularity(tour.TourLogs);
            tour.TourAttributes.ChildFriendliness = ComputeChildFriendliness(tour);
            tour.TourAttributes.SearchAlgorithmRanking = ComputeSearchAlgorithmRanking(tour.TourAttributes.Popularity, tour.TourAttributes.ChildFriendliness);

            _logger.LogInformation("Updating TourAttributes for Tour => {@TourId}.\nPopularity: {Popularity}, ChildFriendliness: {ChildFriendly}, Ranking: {Ranking}",
                tour.Id,
                tour.TourAttributes.Popularity,
                tour.TourAttributes.ChildFriendliness,
                tour.TourAttributes.SearchAlgorithmRanking);

            _tourAttributesRepository.UpdateTourAttributes(tour.TourAttributes);
        }
    }
}
