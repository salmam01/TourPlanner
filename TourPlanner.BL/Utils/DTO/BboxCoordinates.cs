using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.UI.Utils.DTO
{
    public class BboxCoordinates
    {
        public double MinLongitude { get; set; } = 0;
        public double MaxLongitude { get; set; } = 0;
        public double MinLatitude { get; set; } = 0;
        public double MaxLatitude { get; set; } = 0;
    }
}
