using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TourPlanner.Models.Entities
{
    //  TODO: add OR / Mapping
    public class Tour
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string From { get; set; }
        [Required]
        public string To { get; set; }
        [Required]
        public string TransportType { get; set; }
        public double Distance { get; set; }
        public TimeSpan EstimatedTime { get; set; }
        [Required]
        public string RouteInformation { get; set; }
        public TourAttributes TourAttributes { get; set; }
        public ICollection<TourLog> TourLogs { get; set; }

        public Tour()
        {
            TourAttributes = new TourAttributes
            {
                Id = Id
            };
            TourLogs = new List<TourLog>();
            RouteInformation = "";
        }

        public Tour (string name, DateTime date, string description, string transportType, string from, string to)
        {
            Name = name;
            Date = date;
            Description = description;
            TransportType = transportType;
            From = from;
            To = to;
            Distance = 0;
            EstimatedTime = TimeSpan.Zero;
            RouteInformation = "";

            TourAttributes = new TourAttributes
            {
                Id = Id
            };

            TourLogs = new List<TourLog>();
        }
    }
}
