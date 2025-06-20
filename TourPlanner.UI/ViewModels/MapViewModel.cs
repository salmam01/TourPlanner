using System;
using System.Collections.Generic;
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
            _ = GetRouteMap(tour);
        }

        private async Task GetRouteMap(Tour tour)
        {
            //  Get the GeoCoordinates of the start and destination
            //GeoCoordinates startGeoCoordinates = await _openRouteService.GetGeoCoordinatesAsync(tour.From);
            //GeoCoordinates destinationGeoCoordinates = await _openRouteService.GetGeoCoordinatesAsync(tour.To);

            //  Get a list of way points between the start and destination
            //List<double[]> wayPoints = await _openRouteService.GetWayPointsAsync(startGeoCoordinates.Coordinates, destinationGeoCoordinates.Coordinates);

            //  Draw the map using Leaflet: bbox and waypoints ??
        }
    }
}
