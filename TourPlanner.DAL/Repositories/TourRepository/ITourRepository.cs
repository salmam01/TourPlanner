using System;
using System.Collections.Generic;
using TourPlanner.Models.Entities;

namespace TourPlanner.DAL.Repositories.TourRepository;

public interface ITourRepository {

    Tour? GetTourById(Guid id);
    IEnumerable<Tour> GetTours();
    IEnumerable<Tour> SearchTours(string query);
    //IEnumerable<Tour> SearchTours(string query, double? minPopularity, bool? childFriendliness);

    void InsertTour(Tour tour);
    void UpdateTour(Tour tour);
    void DeleteTour(Guid tourId);
    void DeleteAllTours();
    void Save();
}