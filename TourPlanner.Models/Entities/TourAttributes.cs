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
        public double Popularity { get; set; }
        public bool ChildFriendliness { get; set; }
        public double SearchAlgorithmRanking { get; set; }

        // Always ensure a new GUID is set if not specified
        public TourAttributes()
        {
            if (Id == Guid.Empty)
                Id = Guid.NewGuid();
        }
    }
}
