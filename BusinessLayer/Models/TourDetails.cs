using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class TourDetails
    {
        [Key]
        public Guid Id { get; set; }
        public string RouteInformation { get; set; }
        public double Distance { get; set; }
        public TimeSpan EstimatedTime { get; set; }
    }
}
