import { GridDataResult } from '@progress/kendo-angular-grid';
import { LookupKeyAndName } from '../../shared';
import { DateTimeScope, PlaceScope, CrashQuery, QueryRef } from './crash-query';

export class EventAnalysisStateService {
    gridPageSize = 10;
    dateTimeScope = new DateTimeScope();
    placeScope = new PlaceScope();
    crashQuery = new CrashQuery();
    crashQueryRef: QueryRef;
    crashGridData: GridDataResult;
    crashGridSkip = 0;
    allCounties: LookupKeyAndName[];
    allCities: LookupKeyAndName[];
}
