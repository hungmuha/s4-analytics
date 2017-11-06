using Dapper;
using Microsoft.Extensions.Options;
using MoreLinq;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class ReportRepository
    {
        private readonly string _connStr;

        public ReportRepository(
            IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.FlatConnStr;
        }

        public ReportOverTime<int> GetCrashCountsByYear()
        {
            var queryText = $@"WITH grouped_cts AS (
                -- count matching crashes, grouped by year and month
                SELECT
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                -- INSERT FILTERS HERE
                GROUP BY crash_yr, crash_mm
            )
            SELECT -- sum previous counts, grouped by series and year
                CASE WHEN crash_mm < 10 THEN 1 ELSE 2 END AS seq,
                CASE WHEN crash_mm < 10 THEN 'Jan 1 - Sep 30' ELSE 'Oct 1 - Dec 31' END AS series,
                CAST(crash_yr AS VARCHAR2(4)) AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            WHERE (crash_yr < 2017 OR crash_mm < 10)
            GROUP BY CASE WHEN crash_mm < 10 THEN 1 ELSE 2 END, CASE WHEN crash_mm < 10 THEN 'Jan 1 - Sep 30' ELSE 'Oct 1 - Dec 31' END, crash_yr
            ORDER BY CASE WHEN crash_mm < 10 THEN 1 ELSE 2 END, crash_yr";

            var report = new ReportOverTime<int>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new { });
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

        public ReportOverTime<int> GetCrashCountsByMonth()
        {
            var queryText = $@"WITH grouped_cts AS (
                -- count matching crashes, grouped by year and month
                SELECT
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                WHERE crash_yr IN (2017, 2016)
                -- INSERT FILTERS HERE
                GROUP BY crash_yr, crash_mm
            )
            SELECT -- sum previous counts, grouped by series and month
                CAST(crash_yr AS VARCHAR2(4)) AS series,
                CAST(crash_mm AS VARCHAR2(2)) AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            WHERE (crash_yr < 2017 OR crash_mm < 10)
            GROUP BY crash_yr, crash_mm
            ORDER BY crash_yr, crash_mm";

            var report = new ReportOverTime<int>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new { });
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

        public ReportOverTime<int> GetCrashCountsByDay()
        {
            var queryText = $@"-- count matching crashes, grouped by day
                SELECT
                    CAST(crash_yr AS VARCHAR2(4)) AS series,
                    COUNT(*) ct
                FROM crash_evt
                WHERE crash_yr IN (2017, 2016)
                AND (crash_yr < 2017 OR crash_mm < 10)
                -- INSERT FILTERS HERE
                GROUP BY crash_yr, key_crash_dt
                ORDER BY crash_yr, key_crash_dt";

            var report = new ReportOverTime<int>();
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, new { });
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
