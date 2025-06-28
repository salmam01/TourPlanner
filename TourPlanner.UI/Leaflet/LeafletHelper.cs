using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourPlanner.BL.Utils.DTO;

namespace TourPlanner.UI.Leaflet
{
    public class LeafletHelper
    {
        private readonly string _baseDirectory;
        public LeafletHelper(string baseDirectory)
        {
             _baseDirectory = baseDirectory;
        }

        public void SaveMapGeometryToJsFile(MapGeometry mapGeometry)
        {
            var coordinates = mapGeometry.WayPoints
                .Select(wp => $"[{wp.Longitude.ToString(CultureInfo.InvariantCulture)}, {wp.Latitude.ToString(CultureInfo.InvariantCulture)}]");

            string jsContent = $@"
                                var directions = {{
                                    ""type"": ""Feature"",
                                    ""geometry"": {{
                                        ""type"": ""LineString"",
                                        ""coordinates"": [
                                            {string.Join(", \n", coordinates)}
                                        ]
                                    }},
                                    ""bbox"": [{mapGeometry.Bbox.MinLongitude.ToString(CultureInfo.InvariantCulture)}, {mapGeometry.Bbox.MinLatitude.ToString(CultureInfo.InvariantCulture)},
                                               {mapGeometry.Bbox.MaxLongitude.ToString(CultureInfo.InvariantCulture)}, {mapGeometry.Bbox.MaxLongitude.ToString(CultureInfo.InvariantCulture)}]
                                }};";

            string outputPath = Path.Combine(_baseDirectory, "TourPlanner.UI", "Leaflet", "directions.js");

            File.WriteAllText(outputPath, jsContent);
        }
    }
}
