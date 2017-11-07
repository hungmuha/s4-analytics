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

            var queryText = $@"WITH grouped_cts AS (
                -- count matching crashes, grouped by year and month
                SELECT
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                WHERE key_crash_dt < :maxDate + 1
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

        public ReportOverTime<int> GetCrashCountsByMonth(int? year = null)
        {
            // find the last day of the last full month that ended at least MIN_DAYS_BACK days ago
            var nDaysAgo = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
            var maxDate = new DateTime(nDaysAgo.Year, nDaysAgo.Month, 1).Subtract(new TimeSpan(1, 0, 0, 0));

            if (year != null && year < maxDate.Year)
            {
                maxDate = new DateTime((int)year, 12, 31);
            }

            var queryText = $@"WITH grouped_cts AS (
                -- count matching crashes, grouped by year and month
                SELECT
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                WHERE crash_yr IN (:year, :year - 1)
                AND key_crash_dt < :maxDate + 1
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
                    maxDate.Year
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

        public ReportOverTime<int> GetCrashCountsByDay(int? year = null)
        {
            // find the date MIN_DAYS_BACK days ago
            DateTime maxDate = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));

            if (year != null && year < maxDate.Year)
            {
                maxDate = new DateTime((int)year, 12, 31);
            }

            var queryText = $@"-- count matching crashes, grouped by day
                SELECT
                    CAST(crash_yr AS VARCHAR2(4)) AS series,
                    COUNT(*) ct
                FROM crash_evt
                WHERE crash_yr IN (:year, :year - 1)
                AND key_crash_dt < :maxDate + 1
                -- INSERT FILTERS HERE
                GROUP BY crash_yr, key_crash_dt
                ORDER BY crash_yr, key_crash_dt";

            var report = new ReportOverTime<int>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new {
                    maxDate,
                    maxDate.Year
                });
                var seriesNames = results.DistinctBy(r => r.SERIES).Select(r => (string)(r.SERIES));
                var series = new List<ReportSeries<int>>();
                foreach (var seriesName in seriesNames)
                {
                    var seriesData = results.Where(r => r.SERIES == seriesName);
                    series.Add(new ReportSeries<int>()
                    {
                        name = seriesName,
                        data = seriesData.Select(r => (int)r.CT)
                    });
                }
                report.series = series;
            }
            return report;
        }
    }
}
