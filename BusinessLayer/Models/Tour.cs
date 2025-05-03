using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    //  TODO: add OR / Mapping
    public class Tour
    {
        [Key]
        public Guid Id { get; set; }
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
        public TourDetails TourDetails { get; set; }
        public TourAttributes TourAttributes { get; set; }
        public ICollection<TourLog> TourLogs { get; set; }

        public Tour (string name, DateTime date, string description, string transportType, string from, string to)
        {
            Name = name;
            Date = date;
            Description = description;
            TransportType = transportType;
            From = from;
            To = to;
        }
    }
}
