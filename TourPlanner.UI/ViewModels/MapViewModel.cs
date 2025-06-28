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
using TourPlanner.UI.Leaflet;

namespace TourPlanner.UI.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private readonly EventAggregator _eventAggregator;
        private readonly OpenRouteService _openRouteService;
        private readonly LeafletHelper _leafletHelper;
        private Tour _selectedTour;

        public string BaseDirectory { get; }

        public MapViewModel(
            EventAggregator eventAggregator, 
            OpenRouteService openRouteService,
            LeafletHelper leafletHelper,
            string baseDirectory
        ) {
            _eventAggregator = eventAggregator;
            _openRouteService = openRouteService;
            _leafletHelper = leafletHelper;
            BaseDirectory = baseDirectory;

            _eventAggregator.Subscribe<Tour>(OnTourSelected);
        }

        private void OnTourSelected(Tour tour)
        {
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

            //  Draw the map using Leaflet
            _leafletHelper.SaveMapGeometryAsJson(mapGeometry, BaseDirectory);
        }
    }
}
