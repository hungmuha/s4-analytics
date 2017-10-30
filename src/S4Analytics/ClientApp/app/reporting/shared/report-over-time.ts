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

export class ReportSeriesByDay {
    name: string;
    minDate: Date;
    maxDate: Date;
    data: any;
}

export class ReportOverTimeByDay {
    series: ReportSeriesByDay[];
}
