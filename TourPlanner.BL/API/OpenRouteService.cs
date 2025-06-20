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

namespace TourPlanner.BL.API
{
    //  TODO: work on errors & logging!!
    public class OpenRouteService
    {
        private string _openRouteKey;
        private HttpClient _client;
        private Parser _parser;
        private static int _maxResults = 5;
        private static double _focusPointLat;
        private static double _focusPointLon;

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
            _focusPointLat = focusPointLat;
            _focusPointLon = focusPointLon;
        }

        //  TODO: return Result with data instead!
        public async Task<List<string>> GetLocationsSuggestionsAsync(string query)
        {
            List<string> locations = [];
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    Debug.WriteLine("Query null!");
                    return locations;
                }
                Debug.WriteLine(query);

                //  Focus the search on Austria but don't limit it
                string url = ($"geocode/autocomplete?" +
                                $"api_key={_openRouteKey}" +
                                $"&text={query}" +
                                $"&size={_maxResults}" +
                                $"&focus.point.lat={_focusPointLat.ToString(CultureInfo.InvariantCulture)}" +
                                $"&focus.point.lon={_focusPointLon.ToString(CultureInfo.InvariantCulture)}" +
                                $"&layers=address,street");

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    //  Will return a JSON response with coordinates and bbox
                    //  bbox are the coordinates of the bounding box for the map
                    locations = _parser.ParseLocationSuggestions(await response.Content.ReadAsStringAsync());
                }
                else
                {
                    Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during request: {ex.Message}");
            }

            return locations;
        }

        public async Task<Tour> GetTourInformationAsync(Tour tour)
        {
            GeoCoordinates startGeoCoordinates = await GetGeoCoordinatesAsync(tour.From);
            GeoCoordinates endGeoCoordinates = await GetGeoCoordinatesAsync(tour.To);

            if (startGeoCoordinates != null && endGeoCoordinates != null && startGeoCoordinates.Coordinates.Length == 2 && endGeoCoordinates.Coordinates.Length == 2)
            {
                string url = ($"v2/directions/driving-car?" +
                                $"api_key={_openRouteKey}" +
                                $"&start={startGeoCoordinates.Coordinates[0]},{startGeoCoordinates.Coordinates[1]}" +
                                $"&end={endGeoCoordinates.Coordinates[0]},{endGeoCoordinates.Coordinates[1]}");

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    tour.Distance = _parser.ParseDistance(jsonString);
                    tour.EstimatedTime = _parser.ParseDuration(jsonString);
                }
                else
                    Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}");
            }
            else
            {
                Debug.WriteLine("Failed to get valid coordinates for start or end location.");
            }

            return tour;
        }

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

        public async Task<List<double[]>> GetWayPointsAsync(double[] startCoordinates, double[] endCoordinates)
        {
            List<GeoCoordinates> geoCoordinates = [];

            string startCoordinatesX = startCoordinates[0].ToString(CultureInfo.InvariantCulture);
            string startCoordinatesY = startCoordinates[1].ToString(CultureInfo.InvariantCulture);
            string endCoordinatesX = endCoordinates[0].ToString(CultureInfo.InvariantCulture);
            string endCoordinatesY = endCoordinates[1].ToString(CultureInfo.InvariantCulture);

            //  driving-car?
            string url = ($"v2/directions/driving-car?" +
                            $"api_key={_openRouteKey}" +
                            $"&start={startCoordinatesX},{startCoordinatesY}" +
                            $"&end={endCoordinatesX},{endCoordinatesY}");

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
               // geoCoordinates = _parser.ParseWayPoints(jsonString);
            }
            else
                Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}");

            //  Will return coordinates of way points between start and end coordinates
            return null;
        }
    }
}
