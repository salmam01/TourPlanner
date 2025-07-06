using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Repositories.TourLogRepository;
using TourPlanner.Models.Entities;
using Xunit;

namespace TourPlanner.Tests {
    public class TourLogRepositoryTests {
        private readonly Mock<TourPlannerDbContext> _mockContext;
        private readonly Mock<DbSet<TourLog>> _mockTourLogDbSet;
        private readonly TourLogRepository _repository;
        private readonly List<TourLog> _testTourLogs;
        private readonly Guid _testTourId;

        public TourLogRepositoryTests() {
            _mockContext = new Mock<TourPlannerDbContext>();
            _mockTourLogDbSet = new Mock<DbSet<TourLog>>();
            _repository = new TourLogRepository(_mockContext.Object);
            _testTourId = Guid.NewGuid();

            _testTourLogs = new List<TourLog>
            {
                new TourLog(DateTime.Now.AddDays(-2), 3, 5, "Great hike yaayy ", 12.5, TimeSpan.FromHours(2.4))
                {
                    Id = Guid.NewGuid(),
                    TourId = _testTourId
                },
                new TourLog(DateTime.Now.AddDays(-1), 2, 3, "Nice city tour", 8.2, TimeSpan.FromHours(1.2))
                {
                    Id = Guid.NewGuid(),
                    TourId = _testTourId
                },
                new TourLog(DateTime.Now, 4, 4, "Challenging mountain", 15.0, TimeSpan.FromHours(3.5))
                {
                    Id = Guid.NewGuid(),
                    TourId = Guid.NewGuid()
                }
            };
        }


        [Fact]
        public void Save_CallsSaveChanges_ÜberReflection() {
            var saveMethod = typeof(TourLogRepository).GetMethod("Save",
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Assert.NotNull(saveMethod);
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
            saveMethod.Invoke(_repository, null);
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        
    }
} 