import { GridDataResult } from '@progress/kendo-angular-grid';
import { CrashQuery, CrashQueryRef } from './crash-query';

export class EventAnalysisStateService {
    public gridPageSize = 10;
    public crashQuery: CrashQuery;
    public crashQueryRef: CrashQueryRef;
    public crashGridData: GridDataResult;
    public crashGridSkip = 0;
    public startDate: Date;
    public endDate: Date;
}
