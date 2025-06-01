using Microsoft.Extensions.Logging;
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

        public void InsertTourAttributes(TourAttributes tourAttributes)
        {
            _tourAttributesRepository.InsertTourAttributes(tourAttributes);
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
        public bool ComputeChildFriendliness(ICollection<TourLog> logs)
        {
            if (logs == null || logs.Count == 0)
                return false; // No logs, assume not child-friendly

            // Average difficulty: 1-5, threshold <= 2 (easy/moderate)
            double avgDifficulty = logs.Average(l => l.Difficulty);
            // Average distance: threshold < 10km (adjust as needed)
            double avgDistance = logs.Average(l => l.TotalDistance);
            // Average time: threshold < 2.5h (in minutes)
            double avgMinutes = logs.Average(l => l.TotalTime.TotalMinutes);

            return
                avgDifficulty <= 2.0 &&
                avgDistance <= 10.0 &&
                avgMinutes <= 150.0;
        }

        public double ComputeSearchAlgorithmRanking(double Popularity, bool Childfriendliness)
        {
            double SearchAlgorithmRanking = Popularity / 100;

            if(Childfriendliness)
                SearchAlgorithmRanking *= 1.5;

            return Math.Min(SearchAlgorithmRanking, 1.0);
        }

        /// <summary>
        /// Updates a Tour's TourAttributes object based on logs.
        /// </summary>
        public void UpdateAttributes(Tour tour)
        {
            if (tour == null)
            {
                _logger.LogWarning("Tour is null.");
                return;
            }

            tour.TourAttributes.Popularity = ComputePopularity(tour.TourLogs);
            tour.TourAttributes.ChildFriendliness = ComputeChildFriendliness(tour.TourLogs);
            tour.TourAttributes.SearchAlgorithmRanking = ComputeSearchAlgorithmRanking(tour.TourAttributes.Popularity, tour.TourAttributes.ChildFriendliness);

            _tourAttributesRepository.UpdateTourAttributes(tour.TourAttributes);
        }
    }
}
