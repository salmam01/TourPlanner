using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Utils.DTO
{
    public class BboxCoordinates
    {
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }
    }
}
