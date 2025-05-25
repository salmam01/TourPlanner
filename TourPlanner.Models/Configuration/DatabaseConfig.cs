using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models.Configuration
{
    public class DatabaseConfig
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
        public required string Database { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
