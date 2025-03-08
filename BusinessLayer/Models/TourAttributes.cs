using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class TourAttributes
    {
        public double Popularity { get; set; }
        public bool ChildFriendliness { get; set; }
        public double SearchRanking { get; set; } // not sure what to name it yet
    }
}
