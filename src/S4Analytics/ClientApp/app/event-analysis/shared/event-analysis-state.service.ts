import { GridDataResult } from '@progress/kendo-angular-grid';
import { LookupKeyAndName } from '../../shared';
import { EventFeatureSet } from './event-feature-set';
import { DateTimeScope, PlaceScope, CrashQuery, QueryRef } from './crash-query';

export class EventAnalysisStateService {
    dateTimeScope = new DateTimeScope();
    placeScope = new PlaceScope();
    crashQuery = new CrashQuery();
    crashQueryRef: QueryRef;
    crashGridData: GridDataResult;
    crashFeatureSet: EventFeatureSet;
    allCounties: LookupKeyAndName[];
    allCities: LookupKeyAndName[];
}
