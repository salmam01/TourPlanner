using Microsoft.VisualStudio.TestTools.UnitTesting;
using TourPlanner.BusinessLayer.Models;
using System;

namespace TourPlanner.TestLayer
{
    [TestClass]
    public class AdditionalTourTests
    {
        private Tour testTour;

        [TestInitialize]
        public void Setup()
        {
            testTour = new Tour("Test Tour", "2024-03-23", "Test Description", "Vienna", "Salzburg");
        }

        [TestMethod]
        public void TestTourFromLocationIsSet()
        {
            // Assert
            Assert.AreEqual("Vienna", testTour.From);
        }

        [TestMethod]
        public void TestTourToLocationIsSet()
        {
            // Assert
            Assert.AreEqual("Salzburg", testTour.To);
        }

        [TestMethod]
        public void TestTourDateIsSet()
        {
            // Assert
            Assert.AreEqual("2024-03-23", testTour.Date);
        }

     
        [TestMethod]
        public void TestTourTransportTypeDefaultValue()
        {
            // Assert
            Assert.AreEqual('\0', testTour.TransportType);
        }

        [TestMethod]
        public void TestTourRouteInformationIsNullByDefault()
        {
            // Assert
            Assert.IsNull(testTour.RouteInformation);
        }
    }
} 