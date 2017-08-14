using System.Collections.Generic;

namespace S4Analytics.Models
{
    // Members of the ClientOptions class are exposed via REST API to the Angular app.
    // Don't include anything sensitive here, especially passwords.

    public class ClientOptions
    {
        public string Version { get; set; }
        public string BaseUrl { get; set; }
        public string SilverlightBaseUrl { get; set; }
        public bool IsDevelopment { get; set; }
        public Dictionary<string, CoordinateSystem_Client> CoordinateSystems { get; set; }
    }

    public class MapExtent
    {
        public double MinX { get; set; }
        public double MinY { get; set; }
        public double MaxX { get; set; }
        public double MaxY { get; set; }
    }

    public class CoordinateSystem_Client
    {
        public string Type { get; set; }
        public string EpsgCode { get; set; }
        public string Proj4Def { get; set; }
        public MapExtent MapExtent { get; set; }
    }
}
