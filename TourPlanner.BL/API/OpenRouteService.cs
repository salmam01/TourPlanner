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

namespace TourPlanner.BL.API
{
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

            Debug.WriteLine($"key: {_openRouteKey}, endpoint: {endPoint}, focusPoint Lat: {_focusPointLat}, focusPoint Lon: {_focusPointLon}");
        }

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

                //  Focus the search on Austria but don't limit it
                string url = ($"geocode/autocomplete?" +
                                $"api_key={_openRouteKey}" +
                                $"&text={query}" +
                                $"&size={_maxResults}" +
                                $"&focus.point.lat={_focusPointLat.ToString(CultureInfo.InvariantCulture)}" +
                                $"&focus.point.lon={_focusPointLon.ToString(CultureInfo.InvariantCulture)}" +
                                $"&layers=address,street");

                Debug.WriteLine(url);
                Debug.WriteLine(query);

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine("got response!");
                    //  Will return a JSON response with coordinates and bbox
                    //  bbox are the coordinates of the bounding box for the map
                    locations = _parser.ParseLocationSuggestions(await response.Content.ReadAsStringAsync());
                    Debug.WriteLine(locations);
                }
                else
                {
                    Debug.WriteLine($"Request failed with status: {response.StatusCode}, {response.Content}, {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception during request: {ex.Message}");
            }

            return locations;
        }

        //  TODO: return Result with data instead!
        public async Task<GeoCoordinates> GetGeoCoordinatesAsync(string location)
        {
            //  Parameters: key, text [country, city, street, number]
            GeoCoordinates geoCoordinates = new();
            string url = ($"geocode/search?api_key={_openRouteKey}&text={location}");

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                //  Will return a JSON response with coordinates and bbox
                //  bbox are the coordinates of the bounding box for the map
                string jsonString = await response.Content.ReadAsStringAsync();
                geoCoordinates = _parser.ParseGeoCoordinates(jsonString);
            }

            return geoCoordinates;
        }

        public async Task<List<double[]>> GetWayPointsAsync(double[] startCoordinates, double[] endCoordinates)
        {
            //  Parameters: start coordinates, end coordinates
            //  &start=...,...&end=...,...

            //  Will return coordinates of way points between start and end coordinates
            return null;
        }
    }
}
