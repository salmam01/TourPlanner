using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Utils.DTO;

namespace TourPlanner.BL.Utils
{
    public class Parser
    {
        public List<string> ParseLocationSuggestions(string jsonString)
        {
            List<string> locations = [];
            if (jsonString == null || string.IsNullOrEmpty(jsonString))
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
            if (jsonString == null || string.IsNullOrEmpty(jsonString))
            {
                return geoCoordinates;
            }

            //  Dynamic JSON object
            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject != null)
            {
                return geoCoordinates;
            }

            geoCoordinates.Coordinates = jsonObject["geocoding"]["query"]["lang"]["geometry"]["coordinates"].ToObject<double[]>();
            geoCoordinates.Bbox = jsonObject["geocoding"]["query"]["lang"]["bbox"].ToObject<double[]>();

            Debug.WriteLine(geoCoordinates.Coordinates);
            Debug.WriteLine(geoCoordinates.Bbox);

            return geoCoordinates;
        }

        public double ParseDistance(string jsonString)
        {
            if (jsonString == null)
            {
                return 0.0;
            }

            JObject jsonObject = JObject.Parse(jsonString);
            return jsonObject["features"][0]["properties"]["summary"]["distance"].ToObject<double>();
        }

        public TimeSpan ParseDuration(string jsonString)
        {
            if (jsonString == null)
            {
                return TimeSpan.Zero;
            }

            JObject jsonObject = JObject.Parse(jsonString);
            double duration = jsonObject["features"][0]["properties"]["summary"]["duration"].ToObject<double>();

            return TimeSpan.FromSeconds(duration);
        }

        public List<double[]> ParseWayPoints(string jsonString)
        {
            List<double[]> wayPoints = [];
            if (jsonString == null || string.IsNullOrEmpty(jsonString))
            {
                return wayPoints;
            }

            //  Dynamic JSON object
            JObject jsonObject = JObject.Parse(jsonString);
            if (jsonObject != null)
            {
                return wayPoints;
            }

            //  what do here ? idk
            //wayPoints = jsonObject["features"]["properties"]["geometry"]["coordinates"].ToObject<double[]>;

            return wayPoints;
        }
    }
}
