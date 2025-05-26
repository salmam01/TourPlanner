using System;
using System.Collections.Generic;

namespace TourPlanner.BL.Services
{
    public static class TourLogValidator
    {
        public static string ValidateDate(DateTime? date)
        {
            return date == null ? "Date is required" : null;
        }

        public static string ValidateComment(string comment)
        {
            return string.IsNullOrWhiteSpace(comment) ? "Comment is required" : null;
        }

        public static string ValidateDifficulty(int? difficulty)
        {
            return difficulty == null || difficulty < 1 || difficulty > 5 ? "Difficulty must be between 1 and 5" : null;
        }

        public static string ValidateTotalDistance(double? distance)
        {
            return distance == null || distance < 0 ? "Total distance must be positive" : null;
        }

        public static string ValidateTotalTime(TimeSpan? time)
        {
            return time == null || time.Value.TotalMinutes <= 0 ? "Total time must be positive" : null;
        }

        public static string ValidateRating(int? rating)
        {
            return rating == null || rating < 1 || rating > 5 ? "Rating must be between 1 and 5" : null;
        }

        public static Dictionary<string, string> ValidateAll(DateTime? date, string comment, int? difficulty, double? totalDistance, TimeSpan? totalTime, int? rating)
        {
            var errors = new Dictionary<string, string>
            {
                { nameof(date), ValidateDate(date) },
                { nameof(comment), ValidateComment(comment) },
                { nameof(difficulty), ValidateDifficulty(difficulty) },
                { nameof(totalDistance), ValidateTotalDistance(totalDistance) },
                { nameof(totalTime), ValidateTotalTime(totalTime) },
                { nameof(rating), ValidateRating(rating) }
            };
            return errors;
        }
    }
}