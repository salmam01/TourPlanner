using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models.Entities
{
    public class TourAttributes
    {
        [Key]
        public Guid Id { get; set; }
        public int Popularity { get; set; }
        public bool ChildFriendliness { get; set; }
        public double SearchAlgorithmRanking { get; set; }
        public TourAttributes() {
            Popularity = 0;
            ChildFriendliness = false;
            SearchAlgorithmRanking = 0;
        }
    }
}
