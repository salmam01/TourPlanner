namespace TourPlanner.Tests
{
    using Xunit;
    using TourPlanner.UI.Utils.Validators;
    using System;

    public class TourLogValidatorTests
    {
        [Fact]
        public void ValidComment_ShouldReturnNull() {
            string comment = "Test comment";
            var result = TourLogValidator.ValidateComment(comment);
            Assert.Null(result);
        }

        [Fact]
        public void EmptyComment_ShouldReturnErrorMessage() {
            var comment = "";
            var expectedMessage = "Comment is required";
            string? result = TourLogValidator.ValidateComment(comment);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidDate_ShouldReturnNull() {
            DateTime date = DateTime.UtcNow;
            string? result = TourLogValidator.ValidateDate(date);
            Assert.Null(result);
        }

        [Fact]
        public void NullDate_ShouldReturnErrorMessage() {
            DateTime? date = null;
            string expectedMessage = "Date is required";
            var result = TourLogValidator.ValidateDate(date);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidDifficulty_ShouldReturnNull() {
            int difficulty = 3;
            var result = TourLogValidator.ValidateDifficulty(difficulty);
            Assert.Null(result);
        }

        [Fact]
        public void ValidDistance_ShouldReturnNull() {
            double distance = 10.5;
            var result = TourLogValidator.ValidateTotalDistance(distance);
            
            Assert.Null(result);
        }

        [Fact]
        public void NullDistance_ShouldReturnErrorMessage() {
            double? distance = null;
            string expectedMessage = "Total distance must be positive";
            var result = TourLogValidator.ValidateTotalDistance(distance);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void NegativeDistance_ShouldReturnErrorMessage() {
            double distance = -5.0;
            string expectedMessage = "Total distance must be positive";
            var result = TourLogValidator.ValidateTotalDistance(distance);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidTime_ShouldReturnNull() {
            var time = TimeSpan.FromMinutes(30);
            var result = TourLogValidator.ValidateTotalTime(time);
            Assert.Null(result);
        }

        [Fact]
        public void NullTime_ShouldReturnErrorMessage() {
            TimeSpan? time = null;
            string expectedMessage = "Total time must be positive";
            var result = TourLogValidator.ValidateTotalTime(time);
            Assert.Equal(expectedMessage, result);
        }
        

        [Fact]
        public void ValidRating_ShouldReturnNull() {
            int rating = 4;
            var result = TourLogValidator.ValidateRating(rating);
            Assert.Null(result);
        }

        [Fact]
        public void NullRating_ShouldReturnErrorMessage() { //in UI we can't enter a null value, but we can test it
            int? rating = null;
            string expectedMessage = "Rating must be between 1 and 5";
            var result = TourLogValidator.ValidateRating(rating);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void RatingTooLow_ShouldReturnErrorMessage() {
            int rating = 0;
            string expectedMessage = "Rating must be between 1 and 5";
            var result = TourLogValidator.ValidateRating(rating);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void RatingTooHigh_ShouldReturnErrorMessage() {
            int rating = 6;
            string expectedMessage = "Rating must be between 1 and 5";
            var result = TourLogValidator.ValidateRating(rating);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidTourLog_ShouldReturnNoErrors() {
            var date = DateTime.UtcNow;
            string comment = "Great tour!";
            int difficulty = 3;
            double distance = 10.0;
            var time = TimeSpan.FromMinutes(30);
            int rating = 4;
            
            var errors = TourLogValidator.ValidateAll(date, comment, difficulty, distance, time, rating);
            
            Assert.All(errors.Values, v => Assert.True(v == null));
        }

        [Fact]
        public void InvalidTourLog_ShouldReturnAllErrorMessages() {
            DateTime? date = null;
            string comment = "";
            int difficulty = 0;
            double distance = -1;
            var time = TimeSpan.Zero;
            int rating = 0;
            
            var errors = TourLogValidator.ValidateAll(date, comment, difficulty, distance, time, rating);
            
            Assert.Equal("Date is required", errors["date"]);
            Assert.Equal("Comment is required", errors["comment"]);
            Assert.Equal("Difficulty must be between 1 and 5", errors["difficulty"]);
            Assert.Equal("Total distance must be positive", errors["totalDistance"]);
            Assert.Equal("Total time must be positive", errors["totalTime"]);
            Assert.Equal("Rating must be between 1 and 5", errors["rating"]);
        }
    }
} 