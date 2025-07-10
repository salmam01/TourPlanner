using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.UI.API;
using TourPlanner.UI.Utils.DTO;
using TourPlanner.UI.Utils.Helpers;
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

        public string BaseDirectory { get; }

        public bool IsMapVisible => _selectedTour == null ? false : true;

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
                EventHandler(e);
            });
        }

        private void EventHandler(TourEvent tourEvent)
        {
            switch (tourEvent.Type)
            {
                case TourEvent.EventType.Reload:
                    HideMap();
                    break;
                case TourEvent.EventType.SelectTour:
                    OnTourSelected(tourEvent.Tour);
                    break;
                case TourEvent.EventType.TourEdited:
                    OnTourSelected(tourEvent.Tour);
                    break;
                case TourEvent.EventType.TourDeleted:
                    HideMap();
                    break;
                case TourEvent.EventType.AllToursDeleted:
                    HideMap();
                    break;
            }
        }

        private async void OnTourSelected(Tour tour)
        {
            if (_selectedTour != tour && tour != null)
            {
                _selectedTour = tour;
                OnPropertyChanged(nameof(IsMapVisible));

                //  Get the map geometry needed for the map image
                Result result = await _openRouteService.GetMapGeometry(tour);
                MapGeometry mapGeometry = (MapGeometry)result.Data;

                if (mapGeometry != null && mapGeometry.WayPoints.Count > 0 && mapGeometry.Bbox != null)
                {
                    //  Draw the map using Leaflet
                    _leafletHelper.SaveMapGeometryAsJson(mapGeometry, BaseDirectory, tour.From, tour.To);
                    _eventAggregator.Publish(new TourEvent(TourEvent.EventType.MapUpdated));
                }
            }
        }

        private void HideMap()
        {
            _selectedTour = null;
            OnPropertyChanged(nameof(IsMapVisible));
        }
    }
}
