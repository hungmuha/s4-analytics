import { GridDataResult } from '@progress/kendo-angular-grid';
import { CrashQuery, CrashQueryRef } from './crash-query';
import { LookupKeyAndName } from '../../shared';

export class EventAnalysisStateService {
    gridPageSize = 10;
    crashQuery: CrashQuery;
    crashQueryRef: CrashQueryRef;
    crashGridData: GridDataResult;
    crashGridSkip = 0;
    startDate: Date;
    endDate: Date;
    geoExtent: 'Statewide' | 'County' | 'City';
    selectedCounties: LookupKeyAndName[] = [];
    selectedCities: LookupKeyAndName[] = [];
    allCounties: LookupKeyAndName[];
    allCities: LookupKeyAndName[];
}
