using System;
using System.Collections.Generic;
using System.Linq;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.BusinessLayer.Services
{
    public static class TourAttributesService
    {
        /// <summary>
        /// Computes popularity based on the number of logs.
        /// </summary>
        public static double ComputePopularity(ICollection<TourLog> logs)
        {
            return logs?.Count ?? 0;
        }

        /// <summary>
        /// Computes child-friendliness based on average difficulty, time, and distance.
        /// Returns true if child-friendly, false otherwise.
        /// </summary>
        public static bool ComputeChildFriendliness(ICollection<TourLog> logs)
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

        /// <summary>
        /// Updates a Tour's TourAttributes object based on logs.
        /// </summary>
        public static void UpdateAttributes(Tour tour)
        {
            if (tour == null) return;
            if (tour.TourAttributes == null)
                tour.TourAttributes = new TourAttributes();

            tour.TourAttributes.Popularity = ComputePopularity(tour.TourLogs);
            tour.TourAttributes.ChildFriendliness = ComputeChildFriendliness(tour.TourLogs);
        }
    }
}
