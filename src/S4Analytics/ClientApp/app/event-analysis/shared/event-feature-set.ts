// tslint:disable-next-line
import { Point, FeatureCollection } from 'geojson';

export class EventFeatureSet {
    eventType: string; // crash or violation
    featureCount?: number; // total number of matching features in current extent
    featureExtent: {
        minX: number,
        minY: number,
        maxX: number,
        maxY: number
    }; // extent of features in feature set
    queryExtent: {
        minX: number,
        minY: number,
        maxX: number,
        maxY: number
    }; // queried extent for feature set
    isSample: boolean; // more points exist for current extent
    sampleSize?: number; // total number of sampled events (will hover around 10,000 if nonzero)
    sampleMultiplier?: number; // a count of sample points can be multiplied by this number to approximate the actual count
    featureCollection: FeatureCollection<Point>;
}
