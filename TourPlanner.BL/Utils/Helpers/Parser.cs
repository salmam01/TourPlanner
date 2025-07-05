using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Utils.DTO;

namespace TourPlanner.BL.Utils.Helpers
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

            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject == null)
            {
                return locations;
            }

            int locationsSize = jsonObject["features"].Count();
            for (int i = 0; i < locationsSize; i++)
            {
                string location = jsonObject["features"][i]["properties"]["label"].ToString();
                if (!string.IsNullOrWhiteSpace(location)) {
                    locations.Add(location);
                }
            }
            
            return locations;
        }

        //  TODO: refactor this shit plss
        public GeoCoordinates ParseGeoCoordinates(string jsonString)
        {
            GeoCoordinates geoCoordinates = new();
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return geoCoordinates;
            }

            //  Dynamic JSON object
            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject == null)
            {
                return geoCoordinates;
            }

            geoCoordinates.Longitude = jsonObject["features"][0]["geometry"]["coordinates"][0].ToObject<double>();
            geoCoordinates.Latitude = jsonObject["features"][0]["geometry"]["coordinates"][1].ToObject<double>();

            return geoCoordinates;
        }

        public MapGeometry ParseMapGeometry(string jsonString)
        {
            MapGeometry mapGeometry = new();
            if (string.IsNullOrWhiteSpace(jsonString))
            {
                return mapGeometry;
            }

            //  Dynamic JSON object
            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject == null)
            {
                return mapGeometry;
            }

            int wayPointsLength = jsonObject["features"][0]["geometry"]["coordinates"].Count();
            if (wayPointsLength > 0)
            {
                for (int i = 0; i < wayPointsLength; i++)
                {
                    GeoCoordinates wayPointCoordinates = new();
                    wayPointCoordinates.Longitude = jsonObject["features"][0]["geometry"]["coordinates"][i][0].ToObject<double>();
                    wayPointCoordinates.Latitude = jsonObject["features"][0]["geometry"]["coordinates"][i][1].ToObject<double>();
                    mapGeometry.WayPoints.Add(wayPointCoordinates);
                }
            }

            mapGeometry.Bbox = new BboxCoordinates
            {
                MinLongitude = jsonObject["bbox"][0].ToObject<double>(),
                MinLatitude = jsonObject["bbox"][1].ToObject<double>(),
                MaxLongitude = jsonObject["bbox"][2].ToObject<double>(),
                MaxLatitude = jsonObject["bbox"][3].ToObject<double>()
            };

            double duration = jsonObject["features"][0]["properties"]["summary"]["duration"].ToObject<double>();
            mapGeometry.Distance = jsonObject["features"][0]["properties"]["summary"]["distance"].ToObject<double>();
            mapGeometry.Duration = TimeSpan.FromSeconds(duration);

            return mapGeometry;
        }
    }
}
