using Dapper;
using Microsoft.Extensions.Options;
using MoreLinq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    //public class LookupKeyAndName
    //{
    //    public int key;
    //    public string name;
    //}

    public class CitationReportingRepository
    {
        private readonly string _connStr;

        public CitationReportingRepository(
            IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.FlatConnStr;
        }

        public IEnumerable<LookupKeyAndName> GetGeographyLookups()
        {
            var queryText = @"SELECT id AS key, cnty_nm || ' County, FL' AS name
                FROM dim_geography
                WHERE city_cd = 0
                AND cnty_cd <> 68
                UNION ALL
                SELECT id AS key, city_nm || ', FL' AS name
                FROM dim_geography
                WHERE city_cd <> 0
                AND cnty_cd <> 68
                ORDER BY name";
            IEnumerable<LookupKeyAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyAndName>(queryText, new { });
            }
            return results;
        }

        public IEnumerable<LookupKeyAndName> GetAgencyLookups()
        {
            var queryText = @"SELECT id AS key, agncy_nm AS name
                FROM dim_agncy
                ORDER BY agncy_nm";
            IEnumerable<LookupKeyAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyAndName>(queryText, new { });
            }
            return results;
        }

        private class PreparedQuery
        {
            public string queryText;
            public Dictionary<string, object> queryParameters;
            public DynamicParameters DynamicParams
            {
                get
                {
                    var dynamicParams = new DynamicParameters();
                    dynamicParams.AddDict(queryParameters);
                    return dynamicParams;
                }
            }
            public PreparedQuery(string queryText, Dictionary<string, object> queryParameters)
            {
                this.queryText = queryText;
                this.queryParameters = queryParameters;
            }
        }

        public ReportOverTime<int> GetCitationCountsByYear(CitationsOverTimeQuery query)
        {
            string[] monthNames = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            // find the last day of the last full month that ended at least MIN_DAYS_BACK days ago
            var nDaysAgo = DateTime.Now; //.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
            var maxDate = new DateTime(nDaysAgo.Year, nDaysAgo.Month, 1).Subtract(new TimeSpan(1, 0, 0, 0));
            var minDate = new DateTime(maxDate.Year - 4, 1, 1); // include 4 calendar years prior to maxDate
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

            var preparedQuery = PrepareQuery(query);

            var queryText = $@"WITH grouped_cts AS (
                -- count matching citations, grouped by year and month
                SELECT /*+ RESULT_CACHE */ 
                    citation_yr,
                    citation_mm,
                    COUNT(*) ct
                FROM citation
                WHERE key_citation_dt BETWEEN TRUNC(:minDate) AND TRUNC(:maxDate)
                AND ( {preparedQuery.queryText} )
                GROUP BY citation_yr, citation_mm
            )
            SELECT /*+ RESULT_CACHE */  -- sum previous counts, grouped by series and year
                CASE WHEN citation_mm <= :series1EndMonth THEN 1 ELSE 2 END AS seq,
                CASE WHEN citation_mm <= :series1EndMonth THEN :series1Label ELSE :series2Label END AS series,
                CAST(citation_yr AS VARCHAR2(4)) AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            GROUP BY
                CASE WHEN citation_mm <= :series1EndMonth THEN 1 ELSE 2 END,
                CASE WHEN citation_mm <= :series1EndMonth THEN :series1Label ELSE :series2Label END,
                citation_yr
            ORDER BY
                CASE WHEN citation_mm <= :series1EndMonth THEN 1 ELSE 2 END,
                citation_yr";

            var dynamicParams = preparedQuery.DynamicParams;
            dynamicParams.Add(new
            {
                series1EndMonth,
                series1Label,
                series2Label,
                maxDate,
                minDate
            });

            var report = new ReportOverTime<int>() { maxDate = maxDate };
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, dynamicParams);
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

        public ReportOverTime<int> GetCitationCountsByMonth(int year, CitationsOverTimeQuery query)
        {
            // find the last day of the last full month that ended at least MIN_DAYS_BACK days ago
            var nDaysAgo = DateTime.Now; //.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
            var maxDate = new DateTime(nDaysAgo.Year, nDaysAgo.Month, 1).Subtract(new TimeSpan(1, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            var preparedQuery = PrepareQuery(query);

            var queryText = $@"WITH grouped_cts AS (
                -- count matching citations, grouped by year and month
                SELECT /*+ RESULT_CACHE */
                    citation_yr,
                    citation_mm,
                    citation_mo,
                    COUNT(*) ct
                FROM citation
                WHERE citation_yr IN (:year, :year - 1)
                AND key_citation_dt < TRUNC(:maxDate + 1)
                AND ( {preparedQuery.queryText} )
                GROUP BY citation_yr, citation_mm, citation_mo
            )
            SELECT /*+ RESULT_CACHE */ -- sum previous counts, grouped by series and month
                CAST(citation_yr AS VARCHAR2(4)) AS series,
                citation_mo AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            GROUP BY citation_yr, citation_mm, citation_mo
            ORDER BY citation_yr, citation_mm";

            var dynamicParams = preparedQuery.DynamicParams;
            dynamicParams.Add(new
            {
                maxDate,
                maxDate.Year
            });

            var report = new ReportOverTime<int>() { maxDate = maxDate };
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, dynamicParams);
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

        public ReportOverTime<int?> GetCitationCountsByDay(int year, bool alignByWeek, CitationsOverTimeQuery query)
        {
            // TODO: find the date MIN_DAYS_BACK days ago
            DateTime maxDate = DateTime.Now;//.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            string innerQueryText;

            if (alignByWeek)
            {
                innerQueryText = @"SELECT
                    :year AS yr,
                    dd1.evt_dt,
                    :year - 1 AS prev_yr,
                    dd2.evt_dt AS prev_yr_dt
                FROM dim_date dd1
                FULL OUTER JOIN dim_date dd2
                    ON dd2.evt_dt = dd1.prev_yr_dt_align_day_of_wk -- align by day of week
                WHERE ( dd1.evt_yr = :year OR dd2.evt_yr = :year - 1 )
                ORDER BY dd2.evt_dt, dd1.evt_dt";
            }
            else
            {
                innerQueryText = @"SELECT
                    :year AS yr,
                    dd1.evt_dt,
                    :year - 1 AS prev_yr,
                    dd2.evt_dt AS prev_yr_dt
                FROM dim_date dd1
                FULL OUTER JOIN dim_date dd2
                    ON dd2.evt_dt = dd1.prev_yr_dt_align_day_of_mo -- align by day of month
                WHERE ( dd1.evt_yr = :year OR dd2.evt_yr = :year - 1 )
                AND (
                    dd2.evt_dt IS NULL -- INCLUDE null record if current year has feb 29
                    OR dd2.evt_mm <> 2 OR dd2.evt_dd <> 29  -- EXCLUDE feb 29 prior year
                )
                ORDER BY dd1.evt_dt";
            }

            var preparedQuery = PrepareQuery(query);

            var queryText = $@"WITH
            aligned_dts AS (
                SELECT /*+ RESULT_CACHE */
                    ROWNUM AS seq, yr, evt_dt, prev_yr, prev_yr_dt
                FROM ( {innerQueryText} )
            ),
            citation_cts AS (
                SELECT /*+ RESULT_CACHE */
                    key_citation_dt, COUNT(*) AS ct
                FROM citation ce
                WHERE citation_yr BETWEEN :year - 1 AND :year
                AND ( {preparedQuery.queryText} )
                GROUP BY key_citation_dt
            )
            SELECT /*+ RESULT_CACHE */
                TO_CHAR(yr) AS series, seq, evt_dt,
                CASE
                    WHEN evt_dt IS NULL OR evt_dt >= TRUNC(:maxDate + 1) THEN NULL
                    ELSE NVL(ct, 0)
                END AS ct
            FROM (
                SELECT
                    ad.yr, ad.seq, ad.evt_dt, cts.ct
                FROM aligned_dts ad
                LEFT OUTER JOIN citation_cts cts
                    ON cts.key_citation_dt = ad.evt_dt
                UNION ALL
                SELECT
                    ad.prev_yr AS yr, ad.seq, ad.prev_yr_dt AS evt_dt, cts.ct
                FROM aligned_dts ad
                LEFT OUTER JOIN citation_cts cts
                    ON cts.key_citation_dt = ad.prev_yr_dt
            ) res
            ORDER BY yr, seq";

            var dynamicParams = preparedQuery.DynamicParams;
            dynamicParams.Add(new
            {
                maxDate,
                maxDate.Year,
                isLeapYear = DateTime.IsLeapYear(maxDate.Year) ? 1 : 0
            });

            var report = new ReportOverTime<int?>() { maxDate = maxDate };
            using (var conn = new OracleConnection(_connStr))
            {
                var results = conn.Query(queryText, dynamicParams);
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

        private PreparedQuery PrepareQuery(CitationsOverTimeQuery query)
        {
            // initialize where clause and query parameter collections
            var whereClauses = new List<string>();
            var queryParameters = new Dictionary<string, object>();

            // get predicate methods
            var predicateMethods = GetPredicateMethods(query);

            // generate where clause and query parameters for each valid filter
            predicateMethods.ForEach(generatePredicate => {
                (var whereClause, var parameters) = generatePredicate.Invoke();
                if (whereClause != null)
                {
                    whereClauses.Add(whereClause);
                    if (parameters != null)
                    {
                        queryParameters.AddFields(parameters);
                    }
                }
            });

            // join where clauses
            if (whereClauses.Count == 0)
            {
                // prevent the query from breaking if there are no where clauses
                whereClauses.Add("1=1");
            }
            var queryText = "(" + string.Join(")\r\nAND (", whereClauses) + ")";

            return new PreparedQuery(queryText, queryParameters);
        }

        private List<Func<(string, object)>> GetPredicateMethods(CitationsOverTimeQuery query)
        {
            Func<(string, object)>[] predicateMethods =
            {
                () => GenerateGeographyPredicate(query.geographyId),
                () => GenerateReportingAgencyPredicate(query.reportingAgencyId)
            };
            return predicateMethods.ToList();
        }

        private (string whereClause, object parameters) GenerateGeographyPredicate(int? geographyId)
        {
            // test for valid filter
            if (geographyId == null)
            {
                return (null, null);
            }

            var isCounty = geographyId % 100 == 0;

            // define where clause
            var whereClause = isCounty
                ? @"cnty_cd = :geographyId / 100"
                : @"key_geography = :geographyId";

            // define oracle parameters
            var parameters = new { geographyId };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateReportingAgencyPredicate(int? reportingAgencyId)
        {
            // test for valid filter
            if (reportingAgencyId == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(key_agncy = :reportingAgencyId)";

            // define oracle parameters
            var parameters = new
            {
                isFhpTroop = reportingAgencyId > 1 && reportingAgencyId <= 14 ? 1 : 0,
                reportingAgencyId
            };

            return (whereClause, parameters);
        }
    }
}
