using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class EventPointCollection
    {
        public string eventType; // crash or violation
        public bool isSubsetForQuery; // more points exist for current query
        public bool isSampleForExtent; // more points exist for current extent
        public int queryEventCount; // total number of events matching query
        public int? extentEventCount; // total number of matching events in current extent
        public int? sampleEventCount; // total number of sampled events (will hover around 10,000 if nonzero)
        public double? sampleMultiplier;
        public IEnumerable<EventPoint> points;
    }

    public class EventPoint
    {
        public int? eventId;
        public double x;
        public double y;
    }
}
