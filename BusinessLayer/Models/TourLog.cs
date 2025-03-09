using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class TourLog
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Difficulty { get; set; }
        public double Rating { get; set; }
        public string Comment { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }
    }
}
