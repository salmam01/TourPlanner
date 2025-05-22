using System;
using System.Collections.Generic;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.DataLayer.Repositories.TourRepository;

public interface ITourRepository {
    Tour GetTourById(Guid tourId);
    IEnumerable<Tour> GetTours();
    IEnumerable<Tour> SearchTours(string query);

    void InsertTour(Tour tour);
    void UpdateTour(Tour tour);
    void DeleteTour(Guid tourId);
    void Save();
}