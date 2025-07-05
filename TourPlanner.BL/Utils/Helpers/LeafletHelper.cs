using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Utils.DTO;
using System.Text.Json;

namespace TourPlanner.BL.Utils.Helpers
{
    public class LeafletHelper
    {
        private readonly static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        public void SaveMapGeometryAsJson(MapGeometry mapGeometry, string baseDirectory, string fromAddress, string toAddress)
        {
            var directionsObject = new
            {
                type = "Feature",
                geometry = new
                {
                    type = "LineString",
                    coordinates = mapGeometry.WayPoints.Select(wp =>
                        new[] { wp.Longitude, wp.Latitude }).ToArray()
                },
                bbox = new[] { mapGeometry.Bbox.MinLongitude, mapGeometry.Bbox.MinLatitude, mapGeometry.Bbox.MaxLongitude, mapGeometry.Bbox.MaxLatitude },
                fromAddress,
                toAddress
            };

            string directionsJson = $"var directions = {JsonSerializer.Serialize(directionsObject, _jsonOptions)};";
            string outputPath = Path.Combine(baseDirectory, "TourPlanner.UI", "Leaflet", "directions.js");

            File.WriteAllText(outputPath, directionsJson);
        }
    }
}
