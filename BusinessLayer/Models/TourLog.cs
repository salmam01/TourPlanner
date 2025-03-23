using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class TourLog {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Difficulty { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }


        public TourLog(DateTime date, int difficulty, double rating, string comment, double totalDistance, TimeSpan totalTime) {
            Id = Guid.NewGuid();
            Date = date;
            Difficulty = difficulty;
            Rating = rating;
            Comment = comment;
            TotalDistance = totalDistance;
            TotalTime = totalTime;
        }
        public TourLog() {
            Id = Guid.NewGuid();
            Date = DateTime.Now;
            Difficulty = 1;
            Rating = 1;
            Comment = string.Empty;
            TotalDistance = 0;
            TotalTime = TimeSpan.Zero;
        }

    }

}
