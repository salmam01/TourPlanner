using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.API;
using TourPlanner.BL.Utils.DTO;
using TourPlanner.Models.Entities;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class MapViewModel: BaseViewModel
    {
        private readonly EventAggregator _eventAggregator;
        private readonly OpenRouteService _openRouteService;
        private Tour _selectedTour;

        public MapViewModel(EventAggregator eventAggregator, OpenRouteService openRouteService)
        {
            _eventAggregator = eventAggregator;
            _openRouteService = openRouteService;

            _eventAggregator.Subscribe<Tour>(OnTourSelected);
        }

        private void OnTourSelected(Tour tour)
        {
            Debug.WriteLine("hi. me here");
            if (_selectedTour != tour)
            {
                _selectedTour = tour;
                _ = GetRouteMap();
            }
        }

        private async Task GetRouteMap()
        {
            //  Get the map geometry needed for the map image
            MapGeometry mapGeometry = await _openRouteService.GetMapGeometry(_selectedTour);

            Debug.WriteLine($"Way points: {mapGeometry.WayPoints.Count}");
            Debug.WriteLine($"First WayPoint: {mapGeometry.WayPoints.First().Latitude}, {mapGeometry.WayPoints.First().Longitude}");
            Debug.WriteLine($"Last WayPoint: {mapGeometry.WayPoints.Last().Latitude}, {mapGeometry.WayPoints.Last().Longitude}");
            Debug.WriteLine($"Bbox: {mapGeometry.Bbox}");

            //  Draw the map using Leaflet: bbox and waypoints ??

        }
    }
}
