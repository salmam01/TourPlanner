using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models.Configuration
{
    public class ApiConfig
    {
        public string OpenRouteServiceKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public double FocusPointLat { get; set; } = 0.0;
        public double FocusPointLon { get; set;} = 0.0;
        public int MaxResults { get; set; } = 0;
    }
}
