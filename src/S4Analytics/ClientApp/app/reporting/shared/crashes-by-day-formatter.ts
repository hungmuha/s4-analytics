import * as moment from 'moment';

export class CrashesByDayFormatter {
    /*  * Available data are:
        * this.percentage (not shared) / this.points[i].percentage (shared)
        *   Stacked series and pies only. The point's percentage of the total.
        * this.point (not shared) / this.points[i].point (shared)
        *   The point object. The point name, if defined, is available through this.point.name.
        * this.points
        *   In a shared tooltip, this is an array containing all other properties for each point.
        * this.series (not shared) / this.points[i].series (shared)
        *   The series object. The series name is available through this.series.name.
        * this.total (not shared) / this.points[i].total (shared)
        *   Stacked series only. The total value at this point's x value.
        * this.x
        *   The x value. This property is the same regardless of the tooltip being shared or not.
        * this.y (not shared) / this.points[i].y (shared)
        *   The y value.
    */
    points: any[];
    x: number;
    y: number;

    formatAlignedByWeek(): string {
        let dt = new Date(this.x);
        let mt1 = moment(dt);
        let mt2 = mt1.clone().subtract(364, 'days');
        let mts = this.points.length > 1 ? [mt2, mt1] : [mt1];
        let s = `<b>${mt1.toDate()}</b><br />`;
        for (let i = 0; i < this.points.length; i++) {
            let point = this.points[i];
            s += `<span style="color:${point.color}">\u25CF</span> ${point.series.name}: <b>${mts[i].toDate()}, ${point.y}</b><br/>`;
        }
        return s;
    }

    formatAlignedByDate(): string {
        let dt = new Date(this.x);
        let mt1 = moment(dt);
        let mt2 = moment([mt1.year() - 1, mt1.month(), mt1.date()]);
        let mts = this.points.length > 1 ? [mt2, mt1] : [mt1];
        let s = `<b>${mt1.toDate()}</b><br />`;
        for (let i = 0; i < this.points.length; i++) {
            let point = this.points[i];
            s += `<span style="color:${point.color}">\u25CF</span> ${point.series.name}: <b>${mts[i].toDate()}, ${point.y}</b><br/>`;
        }
        return s;
    }
}
