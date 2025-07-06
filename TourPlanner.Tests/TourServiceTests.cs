using TourPlanner.Models.Utils.Helpers;

namespace TourPlanner.Tests
{
    using Xunit;
    using Moq;
    using TourPlanner.UI.Services;
    using TourPlanner.Models.Entities;
    using TourPlanner.DAL.Repositories.TourRepository;
    using TourPlanner.DAL.Repositories.TourAttributesRepository;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using Npgsql;
    using TourPlanner.UI.Utils.Helpers;

    public class TourServiceTests
    {
        private static TourService CreateService(Mock<ITourRepository> repoMock = null) {
            repoMock ??= new Mock<ITourRepository>();
            var attrRepoMock = new Mock<ITourAttributesRepository>();
            var attrLoggerMock = new Mock<ILogger<TourAttributesService>>();
            var tourAttrService = new TourAttributesService(attrRepoMock.Object, attrLoggerMock.Object);
            var loggerMock = new Mock<ILogger<TourService>>();
            return new TourService(repoMock.Object, tourAttrService, loggerMock.Object);
        }

        private static Tour CreateSampleTour() =>
            new Tour { Id = Guid.NewGuid(), Name = "Sample Tour", From = "Vienna", To = "Salzburg" };

        // ----------- NULL Tests -----------

        [Theory]
        [InlineData("Create")]
        [InlineData("Update")]
        [InlineData("Delete")]
        [InlineData("Recalculate")]
        public void Method_WithNullTour_ShouldReturnNullError(string method) {
            TourService service = CreateService();

            Result result = method switch {
                "Create" => service.CreateTour(null),
                "Update" => service.EditTour(null),
                "Delete" => service.DeleteTour(null),
                "Recalculate" => service.RecalculateTourAttributes(null),
                _ => throw new ArgumentException("Invalid method name")
            };

            Assert.Equal(Result.ResultCode.NullError, result.Code);
        }

        // ----------- Exception Tests -----------

        [Theory]
        [InlineData("Create")]
        [InlineData("Update")]
        [InlineData("Delete")]
        public void Method_WithException_ShouldReturnUnknownError(string method) {
            Tour tour = CreateSampleTour();
            Mock<ITourRepository> repo = new Mock<ITourRepository>();

            switch (method) {
                case "Create":
                    repo.Setup(r => r.InsertTour(tour)).Throws(new Exception("Create failed"));
                    break;
                case "Update":
                    repo.Setup(r => r.UpdateTour(tour)).Throws(new Exception("Update failed"));
                    break;
                case "Delete":
                    repo.Setup(r => r.DeleteTour(tour.Id)).Throws(new Exception("Delete failed"));
                    break;
            }

            TourService service = CreateService(repo);

            Result result = method switch {
                "Create" => service.CreateTour(tour),
                "Update" => service.EditTour(tour),
                "Delete" => service.DeleteTour(tour),
                _ => throw new ArgumentException("Invalid method")
            };

            Assert.Equal(Result.ResultCode.UnknownError, result.Code);
        }

        // ----------- Success Tests -----------

        [Theory]
        [InlineData("Create")]
        [InlineData("Update")]
        [InlineData("Delete")]
        public void Method_WithValidTour_ShouldReturnSuccess(string method) {
            Tour tour = CreateSampleTour();
            Mock<ITourRepository> repo = new Mock<ITourRepository>();

            switch (method) {
                case "Create":
                    repo.Setup(r => r.InsertTour(tour));
                    break;
                case "Update":
                    repo.Setup(r => r.UpdateTour(tour));
                    break;
                case "Delete":
                    repo.Setup(r => r.DeleteTour(tour.Id));
                    break;
            }

            TourService service = CreateService(repo);

            Result result = method switch {
                "Create" => service.CreateTour(tour),
                "Update" => service.EditTour(tour),
                "Delete" => service.DeleteTour(tour),
                _ => throw new ArgumentException("Invalid method")
            };

            Assert.Equal(Result.ResultCode.Success, result.Code);
        }

        // ----------- GetAllTours -----------

        [Fact]
        public void GetAllTours_ShouldReturnList() {
            var repo = new Mock<ITourRepository>();
            var tours = new List<Tour> { new Tour(), new Tour() };
            repo.Setup(r => r.GetTours()).Returns(tours);
            var service = CreateService(repo);

            var result = service.GetAllTours();

            Assert.Equal(Result.ResultCode.Success, result.Code);
            Assert.Equal(2, ((List<Tour>)result.Data).Count);
        }

        [Fact]
        public void GetAllTours_WhenEmpty_ShouldReturnEmptyList() {
            var repo = new Mock<ITourRepository>();
            repo.Setup(r => r.GetTours()).Returns(new List<Tour>());
            TourService service = CreateService(repo);

            Result result = service.GetAllTours();

            Assert.Equal(Result.ResultCode.Success, result.Code);
            Assert.Empty((List<Tour>)result.Data);
        }

        // ----------- DeleteAllTours -----------

        [Fact]
        public void DeleteAllTours_ShouldReturnSuccess() {
            var repo = new Mock<ITourRepository>();
            repo.Setup(r => r.DeleteAllTours());
            TourService service = CreateService(repo);

            Result result = service.DeleteAllTours();

            Assert.Equal(Result.ResultCode.Success, result.Code);
        }

        [Fact]
        public void DeleteAllTours_WithException_ShouldReturnUnknownError() {
            Mock<ITourRepository> repo = new Mock<ITourRepository>();
            repo.Setup(r => r.DeleteAllTours()).Throws(new Exception("DB error"));
            TourService service = CreateService(repo);

            Result result = service.DeleteAllTours();

            Assert.Equal(Result.ResultCode.UnknownError, result.Code);
        }

        // ----------- RecalculateTourAttributes -----------

        [Fact]
        public void RecalculateTourAttributes_ShouldReturnSuccess() {
            TourService service = CreateService();
            Tour tour = CreateSampleTour();

            Result result = service.RecalculateTourAttributes(tour);

            Assert.Equal(Result.ResultCode.Success, result.Code);
        }

        // ----------- SearchToursAndLogs -----------

        [Fact]
        public void SearchToursAndLogs_ShouldReturnCombinedList() {
            Mock<ITourRepository> repo = new Mock<ITourRepository>();
            TourService service = CreateService(repo);

            Tour tour1 = new Tour { Id = Guid.NewGuid(), Name = "Mountain Tour" };
            Tour tour2 = new Tour { Id = Guid.NewGuid(), Name = "City Tour" };
            TourLog tourLog = new TourLog { TourId = tour2.Id };

            repo.Setup(r => r.SearchTours("mountain")).Returns(new List<Tour> { tour1 });
            repo.Setup(r => r.GetTourById(tour2.Id)).Returns(tour2);

            Result result = service.SearchToursAndLogs("mountain", new List<TourLog> { tourLog });

            Assert.Equal(Result.ResultCode.Success, result.Code);
            Assert.Equal(2, ((List<Tour>)result.Data).Count);
        }
    }
}
