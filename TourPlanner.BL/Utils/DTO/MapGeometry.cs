using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Utils.DTO
{
    public class MapGeometry
    {
        public List<GeoCoordinates> WayPoints { get; set; } = [];
        public BboxCoordinates? Bbox { get; set; }
        public double Distance { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
