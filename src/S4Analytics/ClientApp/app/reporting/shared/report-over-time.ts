import * as moment from 'moment';

export class ReportSeries
{
    name: string;
    data: number[];
}

export class ReportOverTime
{
    categories: string[];
    series: ReportSeries[];
}
