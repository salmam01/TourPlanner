using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Utils.DTO;

namespace TourPlanner.BL.Utils
{
    public class Parser
    {
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

            return geoCoordinates;
        }

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
    }
}
