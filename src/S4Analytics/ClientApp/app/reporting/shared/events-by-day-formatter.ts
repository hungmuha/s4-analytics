import * as moment from 'moment';

export class EventsByDayFormatter {
    /*  Note that you will never have an actual instance of this class. Instead the format methods
        below will be bound to a context object provided by Highcharts. The context object will have
        more fields than what are defined below, but we only need to define the ones we use
        (to make TypeScript happy).

        Available data are:
        this.percentage (not shared) / this.points[i].percentage (shared)
          Stacked series and pies only. The point's percentage of the total.
        this.point (not shared) / this.points[i].point (shared)
          The point object. The point name, if defined, is available through this.point.name.
        this.points
          In a shared tooltip, this is an array containing all other properties for each point.
        this.series (not shared) / this.points[i].series (shared)
          The series object. The series name is available through this.series.name.
        this.total (not shared) / this.points[i].total (shared)
          Stacked series only. The total value at this point's x value.
        this.x
          The x value. This property is the same regardless of the tooltip being shared or not.
        this.y (not shared) / this.points[i].y (shared)
          The y value.
    */

    points: any[];
    x: number;
    y: number;

    formatAlignedByWeek(): string {
        let utcMs = this.x;
        let utcOffsetMs = moment().utcOffset() * 60 * 1000;
        let localMs = utcMs - utcOffsetMs;
        let dt = new Date(localMs);

        let mt1 = moment(dt);
        let mt2 = mt1.clone().subtract(364, 'days'); // this is the only line that differs from the method below
        let mts = this.points.length > 1 ? [mt2, mt1] : [mt1];

        let formattedText = '';
        for (let i = 0; i < this.points.length; i++) {
            let point = this.points[i];
            let formattedDate = mts[i].format('ddd, MMM D, YYYY');
            formattedText += `<span style="color:${point.color}">\u25CF</span> ${formattedDate}: <b>${point.y}</b><br/>`;
        }
        return formattedText;
    }

    formatAlignedByDate(): string {
        let utcMs = this.x;
        let utcOffsetMs = moment().utcOffset() * 60 * 1000;
        let localMs = utcMs - utcOffsetMs;
        let dt = new Date(localMs);

        let mt1 = moment(dt);
        let mt2 = moment([mt1.year() - 1, mt1.month(), mt1.date()]); // this is the only line that differs from the method above
        let mts = this.points.length > 1 ? [mt2, mt1] : [mt1];

        let formattedText = '';
        for (let i = 0; i < this.points.length; i++) {
            let point = this.points[i];
            let formattedDate = mts[i].format('ddd, MMM D, YYYY');
            formattedText += `<span style="color:${point.color}">\u25CF</span> ${formattedDate}: <b>${point.y}</b><br/>`;
        }
        return formattedText;
    }
}
