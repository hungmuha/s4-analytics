using GeoJSON.Net.Feature;

namespace S4Analytics.Models
{
    public class EventFeatureSet
    {
        public string eventType; // crash or violation
        public int? eventCount; // total number of matching events in current extent
        public bool isSample; // more points exist for current extent
        public int? sampleSize; // total number of sampled events (will hover around 10,000 if nonzero)
        public double? sampleMultiplier; // a count of sample points can be multiplied by this number to approximate the actual count
        public FeatureCollection featureCollection; // GeoJSON feature collection
    }
}
