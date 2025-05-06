using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class TourLog {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int Difficulty { get; set; }
        [Required]
        public double Rating { get; set; }
        [Required]
        public string Comment { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }

        public Guid TourId { get; set; }
        public Tour Tour { get; set; }


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
