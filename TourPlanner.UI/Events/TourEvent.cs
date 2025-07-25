﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.Models.Entities;

namespace TourPlanner.UI.Events
{
    public class TourEvent
    {
        public enum EventType
        {
            Reload,
            SelectTour,
            CreateTour,
            EditTour,
            ExportTour,
            DeleteTour,
            TourEdited,
            TourDeleted,
            AllToursDeleted,
            LogCreated,
            LogDeleted,
            MapUpdated
        }
        public EventType Type { get; }

        public Tour Tour { get; }

        public TourEvent(EventType eventType, Tour tour = null)
        {
            Type = eventType;
            Tour = tour;
        }
    }
}
