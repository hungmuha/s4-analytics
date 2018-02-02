// tslint:disable-next-line
import { Point, FeatureCollection } from 'geojson';

export class EventFeatureSet {
    queryToken: string; // query token used to retrieve feature set
    eventType: string; // crash or violation
    featureCount: number; // total number of matching features in current extent
    featureExtent?: { minX: number, minY: number, maxX: number, maxY: number }; // extent of features in feature set
    isSample: boolean; // more points exist for current extent
    sampleSize?: number; // total number of sampled events (will hover around 10,000 if defined)
    sampleMultiplier?: number; // a count of sample points can be multiplied by this number to approximate the actual count
    featureCollection: FeatureCollection<Point>;

    constructor(queryToken: string, eventType: string) {
        // initialize an empty feature set
        this.queryToken = queryToken;
        this.eventType = eventType;
        this.featureCount = 0;
        this.isSample = false;
        this.featureCollection = {
            features: [],
            type: 'FeatureCollection'
        };
    }
}
