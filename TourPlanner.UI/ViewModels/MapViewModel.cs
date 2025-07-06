using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.API;
using TourPlanner.BL.Utils.DTO;
using TourPlanner.BL.Utils.Helpers;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;
using TourPlanner.UI.Events;

namespace TourPlanner.UI.ViewModels
{
    public class MapViewModel : BaseViewModel
    {
        private readonly EventAggregator _eventAggregator;
        private readonly OpenRouteService _openRouteService;
        private readonly LeafletHelper _leafletHelper;
        private Tour _selectedTour;
        private bool _isMapVisible = true;

        public string BaseDirectory { get; }

        public bool IsMapVisible
        {
            get => _isMapVisible;
            set
            {
                if (_isMapVisible != value)
                {
                    _isMapVisible = value;
                    OnPropertyChanged(nameof(IsMapVisible));
                }
            }
        }

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

            _eventAggregator.Subscribe<TourEvent>(e =>
            {
                if (e.Type == TourEvent.EventType.SelectTour)
                    OnTourSelected(e.Tour);
            });
        }

        private async void OnTourSelected(Tour tour)
        {
            if (_selectedTour != tour && tour != null)
            {
                _selectedTour = tour;

                //  Get the map geometry needed for the map image
                Result result = await _openRouteService.GetMapGeometry(_selectedTour);
                MapGeometry mapGeometry = (MapGeometry)result.Data;

                if (mapGeometry != null && mapGeometry.WayPoints.Count > 0 && mapGeometry.Bbox != null)
                {
                    //  Draw the map using Leaflet
                    _leafletHelper.SaveMapGeometryAsJson(mapGeometry, BaseDirectory, _selectedTour.From, _selectedTour.To);
                    _eventAggregator.Publish(new TourEvent(TourEvent.EventType.MapUpdated));
                }
            }
        }
    }
}
