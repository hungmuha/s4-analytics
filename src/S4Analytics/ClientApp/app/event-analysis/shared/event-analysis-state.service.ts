import { GridDataResult } from '@progress/kendo-angular-grid';
import { LookupKeyAndName } from '../../shared';
import { DateTimeScope, PlaceScope, CrashQuery, QueryRef } from './crash-query';

export class EventAnalysisStateService {
    dateTimeScope = new DateTimeScope();
    placeScope = new PlaceScope();
    crashQuery = new CrashQuery();
    crashQueryRef: QueryRef;
    crashGridData: GridDataResult;
    allCounties: LookupKeyAndName[];
    allCities: LookupKeyAndName[];
}
