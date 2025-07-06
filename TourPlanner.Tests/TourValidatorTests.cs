namespace TourPlanner.Tests
{
    using Xunit;
    using TourPlanner.UI.Validators;
    using System;

    public class TourValidatorTests
    {
        [Fact]
        public void ValidName_ShouldReturnNull() {
            string name = "Mountain Adventure"; //Arrange
            string? result = TourValidator.ValidateName(name);// Act
             //Assert
            Assert.Null(result);
        }

        [Fact]
        public void EmptyName_ShouldReturnErrorMessage() {
            string name = "";
            string expectedMessage = "Name is required";
            var result = TourValidator.ValidateName(name);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidDescription_ShouldReturnNull() {
            string description = "Beautiful mountain with amazing views";
            string? result = TourValidator.ValidateDescription(description);
            Assert.Null(result);
        }

        [Fact]
        public void EmptyDescription_ShouldReturnErrorMessage() {
            string description = "";
            string expectedMessage = "Description is required";
            var result = TourValidator.ValidateDescription(description);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidTransportType_ShouldReturnNull() {
            string transportType = "Car";
            var result = TourValidator.ValidateTransportType(transportType);
            Assert.Null(result);
        }

        [Fact]
        public void EmptyTransportType_ShouldReturnErrorMessage() {
            string transportType = "";
            string expectedMessage = "TransportType is required";
            var result = TourValidator.ValidateTransportType(transportType);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidFrom_ShouldReturnNull() {
            string from = "Vienna";
            string? result = TourValidator.ValidateFrom(from);
            Assert.Null(result);
        }

        [Fact]
        public void EmptyFrom_ShouldReturnErrorMessage() {
            string from = "";
            string expectedMessage = "From is required";
            var result = TourValidator.ValidateFrom(from);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidTo_ShouldReturnNull() {
            string to = "Salzburg";
            var result = TourValidator.ValidateTo(to);
            Assert.Null(result);
        }

        [Fact]
        public void EmptyTo_ShouldReturnErrorMessage() {
            string to = "";
            string expectedMessage = "To is required";
            string result = TourValidator.ValidateTo(to);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void FutureDate_ShouldReturnNull() {
            DateTime date = DateTime.UtcNow.AddDays(1);
            string? result = TourValidator.ValidateDate(date);
            Assert.Null(result);
        }

        [Fact]
        public void PastDate_ShouldReturnErrorMessage() {
            DateTime date = DateTime.UtcNow.AddDays(-1);
            string expectedMessage = "Date must be in the future";
            string result = TourValidator.ValidateDate(date);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void CurrentDate_ShouldReturnErrorMessage() {
            var date = DateTime.UtcNow;
            string expectedMessage = "Date must be in the future";
            string result = TourValidator.ValidateDate(date);
            Assert.Equal(expectedMessage, result);
        }

        [Fact]
        public void ValidTour_ShouldReturnNoErrors() {
            string name = "Mountain Tour";
            string description = "Amazing hike";
            string transportType = "Car";
            string from = "Vienna";
            string to = "Salzburg";
            DateTime date = DateTime.UtcNow.AddDays(1);
            
            var errors = TourValidator.ValidateAll(name, description, transportType, from, to, date);
            
            Assert.All(errors.Values, v => Assert.Null(v));
        }

        [Fact]
        public void InvalidTour_ShouldReturnAllErrorMessages() {
            string name = "";
            string description = "";
            string transportType = "";
            string from = "";
            string to = "";
            var date = DateTime.UtcNow.AddDays(-1);
            
            var errors = TourValidator.ValidateAll(name, description, transportType, from, to, date);
            
            Assert.Equal("Name is required", errors["name"]);
            Assert.Equal("Description is required", errors["description"]);
            Assert.Equal("TransportType is required", errors["transportType"]);
            Assert.Equal("From is required", errors["from"]);
            Assert.Equal("To is required", errors["to"]);
            Assert.Equal("Date must be in the future", errors["date"]);
        }
    }
}