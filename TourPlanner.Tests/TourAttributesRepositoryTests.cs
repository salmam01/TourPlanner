using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moq;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Repositories.TourAttributesRepository;
using TourPlanner.Models.Entities;
using Xunit;

namespace TourPlanner.Tests
{
    public class TourAttributesRepositoryTests
    {
        private readonly Mock<TourPlannerDbContext> _mockContext;
        private readonly Mock<DbSet<TourAttributes>> _mockTourAttributesDbSet;
        private readonly TourAttributesRepository _repository;
        private readonly List<TourAttributes> _testTourAttributes;

        public TourAttributesRepositoryTests() {
            _mockContext = new Mock<TourPlannerDbContext>();
            _mockTourAttributesDbSet = new Mock<DbSet<TourAttributes>>();
            _repository = new TourAttributesRepository(_mockContext.Object);
            
            _testTourAttributes = new List<TourAttributes> {
                new TourAttributes {
                    Id = Guid.NewGuid(),
                    Popularity = 85,
                    ChildFriendliness = true,
                    SearchAlgorithmRanking = 4.2
                },
                new TourAttributes {
                    Id = Guid.NewGuid(),
                    Popularity = 92,
                    ChildFriendliness = false,
                    SearchAlgorithmRanking = 4.8
                },
                new TourAttributes {
                    Id = Guid.NewGuid(),
                    Popularity = 67,
                    ChildFriendliness = true,
                    SearchAlgorithmRanking = 3.9
                }
            };
        }
        
        
        [Fact]
        public void Save_CallsSaveChanges() {
            _mockContext.Setup(c => c.SaveChanges()).Returns(1);
            _repository.Save();
            _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void Save_DbUpdateException_ThrowsException() {
            _mockContext.Setup(c => c.SaveChanges()).Throws(new DbUpdateException("Test exception!"));
            DbUpdateException exception = Assert.Throws<DbUpdateException>(() => _repository.Save());
            Assert.Equal("Test exception!", exception.Message);
        }


    }
} 