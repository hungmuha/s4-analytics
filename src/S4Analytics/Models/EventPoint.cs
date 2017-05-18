using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace S4Analytics.Models
{
    public class EventPoint
    {
        public double x;
        public double y;
        public int? eventId;

        public Feature ToFeature()
        {
            // GeoJSON.Net expects coordinates in Latitude, Longitude (Y, X) order
            // even though this contradicts the GeoJSON spec!
            return new Feature(new Point(new GeographicPosition(latitude: y, longitude: x)), new { id = eventId });
        }
    }
}
