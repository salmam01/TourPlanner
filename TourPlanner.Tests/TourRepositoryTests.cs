/*using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Repositories.TourRepository;
using TourPlanner.Models.Entities;
using Xunit;

namespace TourPlanner.Tests
{
    public class TourRepositoryTests
    {
        private readonly Mock<TourPlannerDbContext> _mockContext;
        private readonly Mock<DbSet<Tour>> _mockTourDbSet;
        private readonly TourRepository _repository;
        private readonly List<Tour> _testTours;

        public TourRepositoryTests() {
            _mockContext = new Mock<TourPlannerDbContext>();
            _mockTourDbSet = new Mock<DbSet<Tour>>();
            _repository = new TourRepository(_mockContext.Object);
            
            _testTours = new List<Tour> {
                new Tour("Mountain Hike", DateTime.Now.AddDays(-1), "Beautiful mountain ", "Foot", "Vienna", "Salzburg")
                {
                    Id = Guid.NewGuid(),
                    Distance = 150,
                    EstimatedTime = TimeSpan.FromHours(3)
                },
                new Tour("City Tour", DateTime.Now, "Explore", "Bike", "Vienna", "Munich")
                {
                    Id = Guid.NewGuid(),
                    Distance = 80.2,
                    EstimatedTime = TimeSpan.FromHours(2)
                }
            };
        }
        
        
        [Fact]
        public void Save_DbUpdateException_ThrowsException() {
            _mockContext.Setup(c => c.SaveChanges()).Throws(new DbUpdateException("Test exception"));
            var exception = Assert.Throws<DbUpdateException>(() => _repository.Save());
            Assert.Equal("Test exception", exception.Message);
        }

        [Fact]
        public void Save_SuccessfulSave_CallsSaveChanges() {
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
            _repository.Save();
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }
    }
} */