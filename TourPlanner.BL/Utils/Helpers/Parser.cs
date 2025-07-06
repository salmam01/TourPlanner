using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.UI.Utils.DTO;

namespace TourPlanner.UI.Utils.Helpers
{
    public class Parser
    {
        public List<string> ParseLocationSuggestions(string jsonString)
        {
            List<string> locations = [];
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return locations;
            }

            //  Dynamic JSON object
            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject == null)
            {
                return locations;
            }

            JArray? features = jsonObject["features"] as JArray;
            if (features == null || features.Count == 0)
            {
                return locations;
            }

            for (int i = 0; i < features.Count; i++)
            {
                string? location = features[i]["properties"]?["label"]?.ToString();
                if (!string.IsNullOrWhiteSpace(location)) {
                    locations.Add(location);
                }
            }
            
            return locations;
        }

        public GeoCoordinates ParseGeoCoordinates(string jsonString)
        {
            GeoCoordinates geoCoordinates = new();
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return geoCoordinates;
            }

            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject == null)
            {
                return geoCoordinates;
            }

            JArray? features = jsonObject["features"] as JArray;
            if (features == null || features.Count == 0)
            {
                return geoCoordinates;
            }

            JArray? coordinates = features[0]["geometry"]?["coordinates"] as JArray;
            if (coordinates == null || coordinates.Count == 0 || coordinates.Count > 2)
            {
                return geoCoordinates;
            }

            geoCoordinates.Longitude = coordinates[0].ToObject<double>();
            geoCoordinates.Latitude = coordinates[1].ToObject<double>();

            return geoCoordinates;
        }

        public MapGeometry ParseMapGeometry(string jsonString)
        {
            MapGeometry mapGeometry = new();
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return mapGeometry;
            }

            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject == null)
            {
                return mapGeometry;
            }

            JArray? features = jsonObject["features"] as JArray;
            if (features == null || features.Count == 0)
            {
                return mapGeometry;
            }

            int? wayPointsLength = features[0]?["geometry"]?["coordinates"]?.Count();
            if (wayPointsLength > 0)
            {
                for (int i = 0; i < wayPointsLength; i++)
                {
                    JArray? wayPointsArray = features[0]?["geometry"]?["coordinates"]?[i] as JArray;
                    if (wayPointsArray == null || wayPointsArray.Count == 0)
                    {
                        return mapGeometry;
                    }

                    GeoCoordinates wayPointCoordinates = new();
                    wayPointCoordinates.Longitude = wayPointsArray[0]?.ToObject<double>() ?? 0;
                    wayPointCoordinates.Latitude = wayPointsArray[1]?.ToObject<double>() ?? 0;
                    mapGeometry.WayPoints.Add(wayPointCoordinates);
                }
            }

            JArray? bboxArray = jsonObject["bbox"] as JArray;
            if (bboxArray != null && bboxArray.Count >= 4)
            {
                mapGeometry.Bbox = new BboxCoordinates
                {
                    MinLongitude = bboxArray[0]?.ToObject<double>() ?? 0,
                    MinLatitude = bboxArray[1]?.ToObject<double>() ?? 0,
                    MaxLongitude = bboxArray[2]?.ToObject<double>() ?? 0,
                    MaxLatitude = bboxArray[3]?.ToObject<double>() ?? 0
                };
            }

            JObject? summary = features[0]?["properties"]?["summary"] as JObject;
            if (summary != null)
            {
                mapGeometry.Distance = summary["distance"]?.ToObject<double>() ?? 0;
                mapGeometry.Duration = TimeSpan.FromSeconds(summary["duration"]?.ToObject<double>() ?? 0);
            }

            return mapGeometry;
        }
    }
}
