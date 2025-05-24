using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Models.Entities;
using TourPlanner.DAL.Data;

namespace TourPlanner.DAL.Repositories.TourRepository;

public class TourRepository : ITourRepository {
    private readonly TourPlannerDbContext _context;

    public TourRepository(TourPlannerDbContext context) {
        _context = context;
    }

    public Tour GetTourById(Guid tourId) {
        return _context.Tours.Find(tourId);
    }

    public IEnumerable<Tour> GetTours() {
        return _context.Tours;
    }

    public IEnumerable<Tour> SearchTours(string query) {
        if (string.IsNullOrWhiteSpace(query))
            return _context.Tours.ToList();

        // Use PostgreSQL Full-Text Search on the "SearchVector" column
        string ftsQuery = query.Trim().Replace(" ", " & ") + ":*";
        var likeQuery = $"%{query.Trim()}%";

        return _context.Tours.FromSqlRaw(
            "SELECT * FROM \"Tours\" " +
            "WHERE \"SearchVector\" @@ to_tsquery('english', {0})",
            ftsQuery
        ).ToList();
    }
    public IEnumerable<Tour> SearchTours(string query, double? minPopularity, bool? childFriendliness)
    {
        // Filter base: Use FTS if query exists, else all tours
        IQueryable<Tour> toursQueryable;
        if (!string.IsNullOrWhiteSpace(query))
        {
            string ftsQuery = query.Trim().Replace(" ", " & ") + ":*";
            toursQueryable = _context.Tours.FromSqlRaw(
                "SELECT * FROM \"Tours\" WHERE \"SearchVector\" @@ to_tsquery('english', {0})", ftsQuery
            );
        }
        else
        {
            toursQueryable = _context.Tours;
        }

        // Join in TourAttributes if not auto-included
        if (minPopularity.HasValue || childFriendliness.HasValue)
        {
            toursQueryable = toursQueryable.Include(t => t.TourAttributes);
        }

        // Filter by popularity (>= minPopularity)
        if (minPopularity.HasValue)
        {
            toursQueryable = toursQueryable.Where(t => t.TourAttributes != null && t.TourAttributes.Popularity >= minPopularity.Value);
        }

        // Filter by child-friendliness (exact true/false match if set)
        if (childFriendliness.HasValue)
        {
            toursQueryable = toursQueryable.Where(t => t.TourAttributes != null && t.TourAttributes.ChildFriendliness == childFriendliness.Value);
        }

        return toursQueryable.ToList();
    }

    public void InsertTour(Tour tour) {
        _context.Tours.Add(tour);
        Save();
    }

    public void UpdateTour(Tour tour) {
        Tour tourToUpdate = _context.Tours.Find(tour.Id);
        if (tourToUpdate == null) return;
        
        tourToUpdate.Name = tour.Name;
        tourToUpdate.Date = tour.Date;
        tourToUpdate.Description = tour.Description;
        tourToUpdate.TransportType = tour.TransportType;
        tourToUpdate.From = tour.From;
        tourToUpdate.To = tour.To;
        
        Save();
    }

    public void DeleteTour(Guid tourId) {
        Tour tour = _context.Tours.Find(tourId);
        _context.Tours.Remove(tour);
        Save();
    }

    public void Save() {
        try {
            _context.SaveChanges();
        }
        catch (DbUpdateException ex) {
            Console.WriteLine("DB Update Failed: " + ex.InnerException?.Message);
            throw;
        }
    }
}