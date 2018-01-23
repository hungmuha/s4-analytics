import { GridDataResult } from '@progress/kendo-angular-grid';
import { DateTimeScope, PlaceScope, CrashQuery, QueryRef } from './crash-query';
import { LookupKeyAndName } from '../../shared';

export class EventAnalysisStateService {
    gridPageSize = 10;
    dateTimeScope = new DateTimeScope();
    placeScope = new PlaceScope();
    crashQuery = new CrashQuery();
    crashQueryRef: QueryRef;
    crashGridData: GridDataResult;
    crashGridSkip = 0;

    geoExtent: 'Statewide' | 'County' | 'City';
    selectedCounties: LookupKeyAndName[] = [];
    selectedCities: LookupKeyAndName[] = [];
    allCounties: LookupKeyAndName[];
    allCities: LookupKeyAndName[];
}
