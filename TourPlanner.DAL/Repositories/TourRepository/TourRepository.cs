using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TourPlanner.Models.Entities;
using TourPlanner.DAL.Data;
using TourPlanner.DAL.Exceptions;
using Npgsql;

namespace TourPlanner.DAL.Repositories.TourRepository;

public class TourRepository : ITourRepository 
{
    private readonly TourPlannerDbContext _context;

    public TourRepository(TourPlannerDbContext context) 
    {
        _context = context;
    }

    public Tour? GetTourById(Guid id)
    {
        try
        {
            return _context.Tours
                    .Include(t => t.TourAttributes)
                    .Include(t => t.TourLogs)
                    .SingleOrDefault(t => t.Id == id);
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while retrieving Tour by ID.", ex);

            else
                throw;
        }
    }

    public IEnumerable<Tour> GetTours() 
    {
        try
        {
            return _context.Tours
                    .Include(t => t.TourAttributes)
                    .Include(t => t.TourLogs)
                    .ToList();
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while retrieving Tours List.", ex);

            else
                throw;
        }
    }

    public IEnumerable<Tour> SearchTours(string query) 
    {
        try
        {
            // Use PostgreSQL Full-Text Search on the "SearchVector" column
            string ftsQuery = query.Trim().Replace(" ", " & ") + ":*";

            return _context.Tours.FromSqlRaw(
                "SELECT * FROM \"Tours\" " +
                "WHERE \"SearchVector\" @@ to_tsquery('english', {0})",
                ftsQuery
            ).ToList();
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while performing Full-Text Search on Tours.", ex);

            else
                throw;
        }
    }

    public void InsertTour(Tour tour) 
    {
        _context.Tours.Add(tour);
        Save();
    }

    public void UpdateTour(Tour tour) 
    {
        Tour? tourToUpdate = _context.Tours.Find(tour.Id);
        if (tourToUpdate == null)
            throw new DatabaseException($"Could not find Tour with ID {tour.Id} to update.");
        
        tourToUpdate.Name = tour.Name;
        tourToUpdate.Date = tour.Date;
        tourToUpdate.Description = tour.Description;
        tourToUpdate.TransportType = tour.TransportType;
        tourToUpdate.From = tour.From;
        tourToUpdate.To = tour.To;
        tourToUpdate.Distance = tour.Distance;
        tourToUpdate.EstimatedTime = tour.EstimatedTime;

        Save();
    }

    public void DeleteTour(Guid tourId) 
    {
        Tour? tour = _context.Tours.Find(tourId);
        if (tour == null) 
            throw new DatabaseException($"Could not find Tour with ID {tourId} to delete.");

        _context.Tours.Remove(tour);
        Save();
    }

    public void DeleteAllTours()
    {
        try
        {
            _context.Tours.ExecuteDelete();
        }
        catch (Exception ex)
        {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while deleting Tour List from Database.", ex);

            else
                throw;
        }
    }

    private void Save() 
    {
        try {
            _context.SaveChanges();
        }
        catch (Exception ex) {
            if (IsDatabaseException(ex))
                throw new DatabaseException("Error while saving Tour Database changes.", ex);

            else 
                throw;
        }
    }

    private bool IsDatabaseException(Exception ex)
    {
        return (ex is DbUpdateException ||
                ex is PostgresException ||
                ex is InvalidOperationException);
    }
}