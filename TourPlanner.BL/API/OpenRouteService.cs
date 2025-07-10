using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using TourPlanner.UI.Utils.DTO;
using TourPlanner.UI.Utils.Helpers;
using TourPlanner.Models.Entities;
using TourPlanner.Models.Utils.Helpers;

namespace TourPlanner.UI.API
{
    public class OpenRouteService
    {
        private readonly string _openRouteKey;
        private readonly HttpClient _client;
        private readonly Parser _parser;
        private readonly GeoCoordinates _focusPoint;
        private readonly int _maxResults;
        private readonly ILogger<OpenRouteService> _logger;

        public OpenRouteService(
            string openRouteKey,
            Parser parser,
            string endPoint,
            double focusPointLat,
            double focusPointLon,
            int maxResults,
            ILogger<OpenRouteService> logger
        )
        {
            _openRouteKey = openRouteKey;
            _client = new();
            _client.BaseAddress = new Uri(endPoint);
            _parser = parser;

            _focusPoint = new();
            _focusPoint.Latitude = focusPointLat;
            _focusPoint.Longitude = focusPointLon;
            _maxResults = maxResults;

            _logger = logger;
        }

        //  Method that sends API request to OpenRoute to retrieve location suggestions, up to maxResults amount
        public async Task<Result> GetLocationSuggestionsAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    _logger.LogWarning("Trying to send an API request with empty query.");
                    return new Result(Result.ResultCode.NullError);
                }

                //  Focus the search on Austria but don't limit it
                string url = ($"geocode/autocomplete?" +
                              $"api_key={_openRouteKey}" +
                              $"&text={query}" +
                              $"&size={_maxResults}" +
                              $"&focus.point.lat={_focusPoint.Latitude.ToString(CultureInfo.InvariantCulture)}" +
                              $"&focus.point.lon={_focusPoint.Longitude.ToString(CultureInfo.InvariantCulture)}" +
                              $"&layers=address,street,venue,locality");

                //  Will return a JSON response with coordinates and bbox
                //  bbox are the coordinates of the bounding box for the map
                HttpResponseMessage response = await _client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Request failed with {StatusCode}.", response.StatusCode);
                    return new Result(Result.ResultCode.ApiError);
                }

                string responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Location suggestions API response: {Response}", responseContent);

                List<string> locations = _parser.ParseLocationSuggestions(responseContent);
                if (locations == null || locations.Count == 0)
                {
                    _logger.LogError("Failed to parse JSON from location suggestions request. Response: {Response}", responseContent);
                    return new Result(Result.ResultCode.ParseError);
                }

                return new Result(Result.ResultCode.Success, locations);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while retrieving location suggestions");
                return new Result(Result.ResultCode.ApiError);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON from location suggestions request.");
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Locations Suggestions request.");
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        //  Method that sends an API request to OpenRoute to retrieve Information about a Tours distance & time
        public async Task<Result> GetTourInformationAsync(Tour tour)
        {
            try
            {
                if (tour == null)
                {
                    _logger.LogWarning("Trying to get tour information with NULL tour.");
                    return new Result(Result.ResultCode.NullError);
                }

                GeoCoordinates start = await GetGeoCoordinatesAsync(tour.From);
                GeoCoordinates end = await GetGeoCoordinatesAsync(tour.To);

                if (!IsValidCoordinates(start) || !IsValidCoordinates(end))
                {
                    _logger.LogError("Failed to get valid coordinates for start or end location.");
                    return new Result(Result.ResultCode.UnknownError);
                }

                string response = await GetDirectionsAsync(start, end, GetProfile(tour.TransportType));

                if (string.IsNullOrWhiteSpace(response))
                {
                    _logger.LogError("Empty response from GetDirectionsAsync for tour: {TourName}", tour.Name);
                    return new Result(Result.ResultCode.UnknownError);
                }

                MapGeometry mapGeometry = _parser.ParseMapGeometry(response);
                if (mapGeometry.Distance <= 0 ||
                    mapGeometry.Duration == TimeSpan.Zero)
                {
                    _logger.LogError("Failed to parse map geometry JSON.");
                    return new Result(Result.ResultCode.ParseError);
                }

                tour.Distance = mapGeometry.Distance;
                tour.EstimatedTime = mapGeometry.Duration;
                return new Result(Result.ResultCode.Success, tour);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while retrieving tour information.");
                return new Result(Result.ResultCode.ApiError);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON from tour information request.");
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during tour information request.");
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        //  Mathod that returns a MapGeometry DTO that can be used to create the map
        public async Task<Result> GetMapGeometry(Tour tour)
        {
            try
            {
                GeoCoordinates start = await GetGeoCoordinatesAsync(tour.From);
                GeoCoordinates end = await GetGeoCoordinatesAsync(tour.To);

                if (!IsValidCoordinates(start) || !IsValidCoordinates(end))
                {
                    _logger.LogError("Failed to get valid coordinates for start or end location.");
                    return new Result(Result.ResultCode.UnknownError);
                }

                string responseJson = await GetDirectionsAsync(start, end, GetProfile(tour.TransportType));
                if (string.IsNullOrWhiteSpace(responseJson))
                {
                    _logger.LogError("Empty response from GetDirectionsAsync for tour: {TourName}", tour.Name);
                    return new Result(Result.ResultCode.UnknownError);
                }

                MapGeometry mapGeometry = _parser.ParseMapGeometry(responseJson);

                if (mapGeometry.WayPoints.Count <= 0 ||
                    mapGeometry.Distance <= 0 ||
                    mapGeometry.Duration == TimeSpan.Zero ||
                    mapGeometry.Bbox == null)
                {
                    _logger.LogError("Failed to parse map geometry JSON.");
                    return new Result(Result.ResultCode.ParseError);
                }

                return new Result(Result.ResultCode.Success, mapGeometry);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while retrieving map geometry.");
                return new Result(Result.ResultCode.ApiError);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON for Map Geometry.");
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during Map Geometry request.");
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        public async Task<Result> CheckIfAddressExists(string location)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(location))
                {
                    _logger.LogWarning("Trying to check address existence with empty location.");
                    return new Result(Result.ResultCode.InvalidAddress);
                }

                GeoCoordinates geoCoordinates = await GetGeoCoordinatesAsync(location);
                if (geoCoordinates == null || !IsValidCoordinates(geoCoordinates))
                {
                    _logger.LogWarning("Invalid address entered: {Location}", location);
                    return new Result(Result.ResultCode.InvalidAddress);
                }

                _logger.LogInformation("Valid address found: {Location} at coordinates ({Lat}, {Lon})",
                    location, geoCoordinates.Latitude, geoCoordinates.Longitude);
                return new Result(Result.ResultCode.Success);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed while checking address existence for: {Location}", location);
                return new Result(Result.ResultCode.ApiError);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to parse JSON while checking address existence for: {Location}", location);
                return new Result(Result.ResultCode.ParseError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception during address existence check for: {Location}", location);
                return new Result(Result.ResultCode.UnknownError);
            }
        }

        //  Helper-Method that sends an API request to OpenRoute to retrieve Geo Coordinates for a specific location
        public async Task<GeoCoordinates> GetGeoCoordinatesAsync(string location)
        {
            try
            {
                GeoCoordinates geoCoordinates = new();
                string url = ($"geocode/search?api_key={_openRouteKey}&text={location}");

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    geoCoordinates = _parser.ParseGeoCoordinates(await response.Content.ReadAsStringAsync());
                }
                else
                    _logger.LogError("Request to retrieve Coordinates failed with {Status}:", response.StatusCode);

                return geoCoordinates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get coordinates for {Location}.", location);
                return new GeoCoordinates();
            }
        }

        //  Helper-Method that sends an API request to OpenRoute to retrieve Route Information between two specific locations
        public async Task<string> GetDirectionsAsync(GeoCoordinates start, GeoCoordinates end, string profile)
        {
            try
            {
                if (start == null || end == null)
                {
                    _logger.LogWarning("Trying to retrieve Route Information with NULL start and end Coordinates.");
                    return string.Empty;
                }

                if (!IsValidCoordinates(start) || !IsValidCoordinates(end))
                {
                    _logger.LogWarning("Invalid coordinates provided: Start({StartLat}, {StartLon}), End({EndLat}, {EndLon})",
                        start.Latitude, start.Longitude, end.Latitude, end.Longitude);
                    return string.Empty;
                }

                string startX = start.Longitude.ToString(CultureInfo.InvariantCulture);
                string startY = start.Latitude.ToString(CultureInfo.InvariantCulture);
                string endX = end.Longitude.ToString(CultureInfo.InvariantCulture);
                string endY = end.Latitude.ToString(CultureInfo.InvariantCulture);

                string url = ($"v2/directions/{profile}" +
                              $"?api_key={_openRouteKey}" +
                              $"&start={startX},{startY}" +
                              $"&end={endX},{endY}");
                _logger.LogDebug("Requesting directions from ({StartLat}, {StartLon}) to ({EndLat}, {EndLon}) with profile {Profile}",
                    start.Latitude, start.Longitude, end.Latitude, end.Longitude, profile);

                HttpResponseMessage response = await _client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        _logger.LogDebug("Successfully retrieved directions response");
                        return content;
                    }
                    else
                    {
                        _logger.LogWarning("Empty response content from directions API.");
                        return string.Empty;
                    }
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Request failed with {Status}: {ErrorContent}", response.StatusCode, errorContent);
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Request to get Route Information failed.");
                return string.Empty;
            }
        }

        private bool IsValidCoordinates(GeoCoordinates geoCoordinates)
        {
            return geoCoordinates != null
                && geoCoordinates.Latitude >= -90 && geoCoordinates.Latitude <= 90
                && geoCoordinates.Longitude >= -180 && geoCoordinates.Longitude <= 180
                && !(geoCoordinates.Latitude == 0 && geoCoordinates.Longitude == 0); // Exclude null island
        }

        //  Helper method that returns the OpenRoute API profile equivalent
        private string GetProfile(string profile)
        {
            switch (profile)
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
