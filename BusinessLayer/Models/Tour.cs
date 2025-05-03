using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    //  TODO: add OR / Mapping
    public class Tour
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string TransportType { get; set; }
        public string RouteInformation { get; set; }
        public double Distance { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        public List<TourLog> TourLogs { get; set; }

        public Tour (string name, DateTime date, string description, string transportType, string from, string to)
        {
            Name = name;
            Date = date;
            Description = description;
            TransportType = transportType;
            From = from;
            To = to;
        }

        public Tour () 
        {
            TourLogs = new List<TourLog>();
        }
    }
}
