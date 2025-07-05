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
        public BboxCoordinates? Bbox { get; set; } = null;
        public double Distance { get; set; } = 0;
        public TimeSpan Duration { get; set; } = TimeSpan.Zero;
    }
}
