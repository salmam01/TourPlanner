﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models.Configuration
{
    public class DatabaseConfig
    {
        public required string ConnectionString { get; set; } = string.Empty;
    }
}
