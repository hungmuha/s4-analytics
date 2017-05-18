import { Point, FeatureCollection } from 'geojson';

export class EventFeatureSet {
    eventType: string; // crash or violation
    eventCount?: number; // total number of matching events in current extent
    isSample: boolean; // more points exist for current extent
    sampleSize?: number; // total number of sampled events (will hover around 10,000 if nonzero)
    sampleMultiplier?: number; // a count of sample points can be multiplied by this number to approximate the actual count
    featureCollection: FeatureCollection<Point>;
}
