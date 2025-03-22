using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class Tour
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public DateTime Date { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public char TransportType { get; set; }
        public string RouteInformation { get; set; }
        public double Distance { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        public List<TourLog> TourLogs { get; set; }

        public Tour(string name, string date, string description, string from, string to)
        {
            Name = name;
            Date = date;
            Description = description;
            From = from;
            To = to;
        }
    }
}
