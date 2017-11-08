using Dapper;
using Microsoft.Extensions.Options;
using MoreLinq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class ReportRepository
    {
        private const int MIN_DAYS_BACK = 15;
        private readonly string _connStr;

        public ReportRepository(
            IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.FlatConnStr;
        }

        public ReportOverTime<int> GetCrashCountsByYear()
        {
            string[] monthNames = new[] { "","Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };

            // find the last day of the last full month that ended at least MIN_DAYS_BACK days ago
            var nDaysAgo = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
            var maxDate = new DateTime(nDaysAgo.Year, nDaysAgo.Month, 1).Subtract(new TimeSpan(1, 0, 0, 0));
            bool isFullYear = maxDate.Month == 12;

            int series1StartMonth = 1;
            int series1EndMonth = maxDate.Month;
            string series1Format = series1StartMonth == series1EndMonth
                ? "{0}" // single-month series
                : "{0} - {1}"; // multiple-month series
            string series1Label = string.Format(series1Format, monthNames[series1StartMonth], monthNames[series1EndMonth]);

            int? series2StartMonth;
            int? series2EndMonth;
            string series2Label = "";
            if (!isFullYear)
            {
                series2StartMonth = maxDate.Month + 1;
                series2EndMonth = 12;
                string series2Format = series2StartMonth == series2EndMonth
                    ? "{0}" // single-month series
                    : "{0} - {1}"; // multiple-month series
                series2Label = string.Format(series2Format, monthNames[(int)series2StartMonth], monthNames[(int)series2EndMonth]);
            }

            var queryText = @"WITH grouped_cts AS (
                -- count matching crashes, grouped by year and month
                SELECT
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                WHERE key_crash_dt < TRUNC(:maxDate + 1)
                -- INSERT FILTERS HERE
                GROUP BY crash_yr, crash_mm
            )
            SELECT -- sum previous counts, grouped by series and year
                CASE WHEN crash_mm <= :series1EndMonth THEN 1 ELSE 2 END AS seq,
                CASE WHEN crash_mm <= :series1EndMonth THEN :series1Label ELSE :series2Label END AS series,
                CAST(crash_yr AS VARCHAR2(4)) AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            GROUP BY
                CASE WHEN crash_mm <= :series1EndMonth THEN 1 ELSE 2 END,
                CASE WHEN crash_mm <= :series1EndMonth THEN :series1Label ELSE :series2Label END,
                crash_yr
            ORDER BY
                CASE WHEN crash_mm <= :series1EndMonth THEN 1 ELSE 2 END,
                crash_yr";

            var report = new ReportOverTime<int>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new {
                    series1EndMonth,
                    series1Label,
                    series2Label,
                    maxDate
                });
                report.categories = results.DistinctBy(r => r.CATEGORY).Select(r => (string)(r.CATEGORY));
                var seriesNames = results.DistinctBy(r => r.SERIES).Select(r => (string)(r.SERIES));
                var series = new List<ReportSeries<int>>();
                foreach (var seriesName in seriesNames)
                {
                    series.Add(new ReportSeries<int>()
                    {
                        name = seriesName,
                        data = results.Where(r => r.SERIES == seriesName).Select(r => (int)r.CT)
                    });
                }
                report.series = series;
            }
            return report;
        }

        public ReportOverTime<int> GetCrashCountsByMonth(int year, bool yearOnYear = false)
        {
            // find the last day of the last full month that ended at least MIN_DAYS_BACK days ago
            var nDaysAgo = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
            var maxDate = new DateTime(nDaysAgo.Year, nDaysAgo.Month, 1).Subtract(new TimeSpan(1, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            var queryText = @"WITH grouped_cts AS (
                -- count matching crashes, grouped by year and month
                SELECT
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                WHERE (crash_yr = :year OR (:yearOnYear = 1 AND crash_yr = :year - 1))
                AND key_crash_dt < TRUNC(:maxDate + 1)
                -- INSERT FILTERS HERE
                GROUP BY crash_yr, crash_mm
            )
            SELECT -- sum previous counts, grouped by series and month
                CAST(crash_yr AS VARCHAR2(4)) AS series,
                CAST(crash_mm AS VARCHAR2(2)) AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            GROUP BY crash_yr, crash_mm
            ORDER BY crash_yr, crash_mm";

            var report = new ReportOverTime<int>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new {
                    maxDate,
                    maxDate.Year,
                    yearOnYear = yearOnYear ? 1 : 0
                });
                report.categories = results.DistinctBy(r => r.CATEGORY).Select(r => (string)(r.CATEGORY));
                var seriesNames = results.DistinctBy(r => r.SERIES).Select(r => (string)(r.SERIES));
                var series = new List<ReportSeries<int>>();
                foreach (var seriesName in seriesNames)
                {
                    series.Add(new ReportSeries<int>()
                    {
                        name = seriesName,
                        data = results.Where(r => r.SERIES == seriesName).Select(r => (int)r.CT)
                    });
                }
                report.series = series;
            }
            return report;
        }

        public ReportOverTime<int?> GetCrashCountsByDay(int year, bool yearOnYear = true, bool alignByWeek = true)
        {
            // find the date MIN_DAYS_BACK days ago
            DateTime maxDate = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            string innerQueryText;

            if (!yearOnYear)
            {
                innerQueryText = @"SELECT
                    :year AS yr,
                    dd1.evt_dt,
                    NULL AS prev_yr,
                    NULL AS prev_yr_dt
                FROM dim_date dd1
                WHERE dd1.evt_yr = :year
                ORDER BY dd1.evt_dt";
            }
            else if (alignByWeek)
            {
                innerQueryText = @"SELECT
                    :year AS yr,
                    dd1.evt_dt,
                    :year - 1 AS prev_yr,
                    dd2.evt_dt AS prev_yr_dt
                FROM dim_date dd2
                FULL OUTER JOIN dim_date dd1
                    ON dd1.prev_yr_dt_align_day_of_wk = dd2.evt_dt -- align by day of week
                WHERE dd1.evt_yr = :year
                OR dd2.evt_yr = :year - 1
                ORDER BY dd2.evt_dt, dd1.evt_dt";
            }
            else
            {
                innerQueryText = @"SELECT
                    :year AS yr,
                    dd1.evt_dt,
                    :year - 1 AS prev_yr,
                    dd2.evt_dt AS prev_yr_dt
                FROM dim_date dd2
                FULL OUTER JOIN dim_date dd1
                    ON dd1.prev_yr_dt_align_day_of_mo = dd2.evt_dt -- align by day of month
                WHERE dd1.evt_yr = :year
                OR dd2.evt_yr = :year - 1
                ORDER BY CASE -- ensure that NULL value for Feb 29 doesn't sort to the bottom
                    WHEN :isLeapYear = 1 THEN dd1.evt_dt
                    ELSE dd2.evt_dt
                END";
            }

            var queryText = $@"WITH aligned_dts AS (
                SELECT /*+ RESULT_CACHE */
                    ROWNUM AS seq, yr, evt_dt, prev_yr, prev_yr_dt
                FROM ( {innerQueryText} )
            )
            SELECT /*+ RESULT_CACHE */
                series,
                seq,
                evt_dt,
                CASE WHEN evt_dt IS NULL OR evt_dt >= TRUNC(:maxDate + 1) THEN NULL ELSE ct END AS ct
            FROM (
                SELECT TO_CHAR(ad.yr) AS series, ad.seq, ad.evt_dt, COUNT(*) ct
                FROM aligned_dts ad
                LEFT OUTER JOIN crash_evt ce
                    ON ce.key_crash_dt = ad.evt_dt
                GROUP BY ad.seq, ad.yr, ad.evt_dt
                UNION ALL
                SELECT TO_CHAR(ad.prev_yr) AS series, ad.seq, ad.prev_yr_dt AS evt_dt, COUNT(*) ct
                FROM aligned_dts ad
                LEFT OUTER JOIN crash_evt ce
                    ON ce.key_crash_dt = ad.prev_yr_dt
                WHERE :yearOnYear = 1
                GROUP BY ad.seq, ad.prev_yr, ad.prev_yr_dt
            ) res
            ORDER BY series, seq";

            var report = new ReportOverTime<int?>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new {
                    maxDate,
                    maxDate.Year,
                    isLeapYear = DateTime.IsLeapYear(maxDate.Year) ? 1 : 0,
                    yearOnYear = yearOnYear ? 1 : 0
                });
                var seriesNames = results.DistinctBy(r => r.SERIES).Select(r => (string)(r.SERIES));
                var series = new List<ReportSeries<int?>>();
                foreach (var seriesName in seriesNames)
                {
                    var seriesData = results.Where(r => r.SERIES == seriesName);
                    series.Add(new ReportSeries<int?>()
                    {
                        name = seriesName,
                        data = seriesData.Select(r => (int?)r.CT)
                    });
                }
                report.series = series;
            }
            return report;
        }
    }
}
