using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using TourPlanner.BL.Utils;
using TourPlanner.BL.Utils.DTO;
using System.Globalization;
using TourPlanner.Models.Entities;
using Microsoft.Extensions.Logging;

namespace TourPlanner.BL.API
{
    //  TODO: work on errors & logging!!
    public class OpenRouteService
    {
        private readonly string _openRouteKey;
        private readonly HttpClient _client;
        private readonly Parser _parser;
        private readonly GeoCoordinates _focusPoint;
        private readonly static int _maxResults = 5;

        public OpenRouteService(
            string openRouteKey, 
            Parser parser, 
            string endPoint,
            double focusPointLat,
            double focusPointLon
        ) {
            _openRouteKey = openRouteKey;
            _client = new();
            _client.BaseAddress = new Uri(endPoint);
            _parser = parser;

            _focusPoint = new();
            _focusPoint.Latitude = focusPointLat;
            _focusPoint.Longitude = focusPointLon;
        }

        //  Method that sends API request to OpenRoute to retrieve location suggestions, up to maxResults amount
        //  TODO: return Result with data instead!
        public async Task<List<string>> GetLocationSuggestionsAsync(string query)
        {
            List<string> locations = [];
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    Debug.WriteLine("Query null!");
                    return locations;
                }

                //  Focus the search on Austria but don't limit it
                string url = ($"geocode/autocomplete?" +
                              $"api_key={_openRouteKey}" +
                              $"&text={query}" +
                              $"&size={_maxResults}" +
                              $"&focus.point.lat={_focusPoint.Latitude.ToString(CultureInfo.InvariantCulture)}" +
                              $"&focus.point.lon={_focusPoint.Longitude.ToString(CultureInfo.InvariantCulture)}" +
                              $"&layers=address,street");

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    //  Will return a JSON response with coordinates and bbox
                    //  bbox are the coordinates of the bounding box for the map
                    locations = _parser.ParseLocationSuggestions(await response.Content.ReadAsStringAsync());
                }
                else
                    Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during request: {ex.Message}");
            }

            return locations;
        }

        //  Method that sends an API request to OpenRoute to retrieve Information about a Tours distance & time
        public async Task<Tour> GetTourInformationAsync(Tour tour)
        {
            GeoCoordinates start = await GetGeoCoordinatesAsync(tour.From);
            GeoCoordinates end = await GetGeoCoordinatesAsync(tour.To);

            if (start != null && end != null 
                && start.Longitude > 0 && start.Latitude > 0 
                && end.Longitude > 0 && end.Latitude > 0
            ) {
                string response = await GetDirectionsAsync(start, end, GetProfile(tour.TransportType));

                if (!string.IsNullOrWhiteSpace(response))
                {
                    MapGeometry mapGeometry = _parser.ParseMapGeometry(response);

                    tour.Distance = mapGeometry.Distance;
                    tour.EstimatedTime = mapGeometry.Duration;
                }
            }
            else
                Debug.WriteLine("Failed to get valid coordinates for start or end location.");

            return tour;
        }

        public async Task<MapGeometry> GetMapGeometry(Tour tour)
        {
            MapGeometry mapGeometry = new();
            GeoCoordinates start = await GetGeoCoordinatesAsync(tour.From);
            GeoCoordinates end = await GetGeoCoordinatesAsync(tour.To);

            // directions
            string responseJson = await GetDirectionsAsync(start, end, GetProfile(tour.TransportType));
            if (!string.IsNullOrWhiteSpace(responseJson))
                mapGeometry = _parser.ParseMapGeometry(responseJson);

            return mapGeometry;
        }

        //  Helper-Method that sends an API request to OpenRoute to retrieve Geo Coordinates for a specific location
        //  TODO: return Result with data instead!
        public async Task<GeoCoordinates> GetGeoCoordinatesAsync(string location)
        {
            GeoCoordinates geoCoordinates = new();
            string url = ($"geocode/search?api_key={_openRouteKey}&text={location}");

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
                geoCoordinates = _parser.ParseGeoCoordinates(await response.Content.ReadAsStringAsync());
            else
                Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}");

            return geoCoordinates;
        }

        //  Helper-Method
        public async Task<string> GetDirectionsAsync(GeoCoordinates start, GeoCoordinates end, string profile)
        {
            string result = "";
            if (start != null && end != null)
            {
                string startX = start.Longitude.ToString(CultureInfo.InvariantCulture);
                string startY = start.Latitude.ToString(CultureInfo.InvariantCulture);
                string endX = end.Longitude.ToString(CultureInfo.InvariantCulture);
                string endY = end.Latitude.ToString(CultureInfo.InvariantCulture);

                //  driving-car?
                string url = ($"v2/directions/{profile}" +
                              $"?api_key={_openRouteKey}" +
                              $"&start={startX},{startY}" +
                              $"&end={endX},{endY}");

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    result = await response.Content.ReadAsStringAsync();
                else
                    Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}");
            }

            return result;
        }

        //  Helper method that returns the OpenRoute API profile equivalent
        private string GetProfile(string profile)
        {
            switch(profile)
            {
                case "Walking":
                    return "foot-walking";
                case "Hiking (Trails)":
                    return "foot-hiking";
                case "Bicycle":
                    return "cycling-regular";
                case "Road Bike":
                    return "cycling-road";
                case "Mountain Bike":
                    return "cycling-mountain";
                case "E-Bike":
                    return "cycling-electric";
                default: return "driving-car";
            }
        }

    }
}
