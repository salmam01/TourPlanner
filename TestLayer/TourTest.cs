using Microsoft.VisualStudio.TestTools.UnitTesting;
using TourPlanner.BusinessLayer.Models;
using System;

namespace TourPlanner.TestLayer
{
    [TestClass]
    public class TourTests
    {
        private Tour testTour;

        [TestInitialize]
        public void Setup()
        {
            testTour = new Tour
            {
                Name = "Test Tour",
                Description = "Prob:description",
                Distance = 10.5,
                EstimatedTime = TimeSpan.FromHours(2)
            };
        }

        [TestMethod]
        public void TestTourNameIsCorrect()
        {
                     
            // Assert
            Assert.AreEqual("Test Tour", testTour.Name);
        }

        [TestMethod]
        public void TestTourDescriptionIsCorrect()
        {
            // Assert
            Assert.AreEqual("Prob:description", testTour.Description);
        }

        [TestMethod]
        public void TestTourDistanceIsPositive()
        {
            // Assert
            Assert.IsTrue(testTour.Distance > 0);
        }

        [TestMethod]
        public void TestEstimatedTimeIsValid()
        {
            // Assert
            Assert.AreEqual(TimeSpan.FromHours(2), testTour.EstimatedTime);
        }

        [TestMethod]
        public void TestTourLogsInitiallyEmpty()
        {
            // Assert
            Assert.IsNotNull(testTour.TourLogs);
            Assert.AreEqual(0, testTour.TourLogs.Count);
        }
    }
}