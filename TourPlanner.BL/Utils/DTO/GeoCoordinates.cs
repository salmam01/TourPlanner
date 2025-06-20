using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Utils.DTO
{
    public class GeoCoordinates
    {
        public double[] Coordinates { get; set; } = [];
        public double[] Bbox { get; set; } = [];
    }
}
