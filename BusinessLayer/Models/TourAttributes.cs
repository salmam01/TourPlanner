using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BusinessLayer.Models
{
    public class TourAttributes
    {
        [Key]
        public Guid Id { get; set; }
        public double Popularity { get; set; }
        public bool ChildFriendliness { get; set; }
        public double SearchAlgorithmRanking { get; set; }

    }
}
