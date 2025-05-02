using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BusinessLayer.Models;

namespace TourPlanner.BusinessLayer.Services
{
    public class TourLogService
    {
        public void CreateTourLog(Tour tour, TourLog tourLog)
        {
            /*
            ValidateTourLogData(date, comment, difficulty, rating, totalDistance, totalTime);

            TourLog tourLog = new TourLog
            {
                Id = Guid.NewGuid(),
                Date = date,
                Comment = comment,
                Difficulty = difficulty,
                Rating = rating,
                TotalDistance = totalDistance,
                TotalTime = totalTime
            };*/

            if (tour.TourLogs == null)
            {
                tour.TourLogs = new List<TourLog>();
            }
            tour.TourLogs.Add(tourLog);
        }

        public void UpdateTourLog(TourLog tourLog, DateTime date, string comment, int difficulty, double rating, double totalDistance, TimeSpan totalTime)
        {
            //ValidateTourLogData(date, comment, difficulty, rating, totalDistance, totalTime);

            tourLog.Date = date;
            tourLog.Comment = comment;
            tourLog.Difficulty = difficulty;
            tourLog.Rating = rating;
            tourLog.TotalDistance = totalDistance;
            tourLog.TotalTime = totalTime;
        }

        public void DeleteTourLog(TourLog tourLog, Tour tour) {
            tour.TourLogs?.Remove(tourLog);
        }

        public List<TourLog> GetTourLogs(Tour tour)
        {
            return tour.TourLogs ?? new List<TourLog>();
        }

        //Diese Fehlermeldungen sind noch nicht sichtbar;  wenn die daten nicht stimmen kann man nicht auf create drücken(box ist grau), TODO: Fehlermeldungen anzeigen
        /* These are a little unnecessary because you already validate in TourLogManagement VM
        private void ValidateTourLogData(DateTime date, string comment, int difficulty, double rating, double totalDistance, TimeSpan totalTime)
        {
            if (date > DateTime.Now)
                throw new ArgumentException("Das Datum kann nicht in der Zukunft liegen.");

            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Kommentar darf nicht leer sein.");

            if (difficulty < 1 || difficulty > 5)
                throw new ArgumentException("Schwierigkeit muss zwischen 1 und 5 liegen.");

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Bewertung muss zwischen 1 und 5 liegen.");

            if (totalDistance < 0)
                throw new ArgumentException("Distanz muss größer oder gleich 0 sein.");

            if (totalTime.TotalMinutes < 0)
                throw new ArgumentException("Zeit muss größer oder gleich 0 sein.");
        }*/
    }
}
