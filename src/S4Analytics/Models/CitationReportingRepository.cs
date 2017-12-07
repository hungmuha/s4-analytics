using Dapper;
using Microsoft.Extensions.Options;
using MoreLinq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class CitationReportingRepository: ReportingRepository
    {
        private const int MIN_DAYS_BACK = 15;

        public CitationReportingRepository(IOptions<ServerOptions> serverOptions) : base(serverOptions)
        {
        }

        public ReportOverTime<int> GetCitationCountsByYear(CitationsOverTimeQuery query)
        {
            string[] monthNames = new[] { "", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            // find the last day of the last full month that ended at least MIN_DAYS_BACK days ago
            var nDaysAgo = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
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

            var preparedWhereClause = PrepareWhereClause(query);

            var queryText = $@"WITH grouped_cts AS (
                -- count matching citations, grouped by year and month
                SELECT /*+ RESULT_CACHE */ 
                    citation_yr,
                    citation_mm,
                    COUNT(*) ct
                FROM citation
                WHERE key_citation_dt BETWEEN TRUNC(:minDate) AND TRUNC(:maxDate)
                AND ( {preparedWhereClause.whereClauseText} )
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

            var dynamicParams = preparedWhereClause.DynamicParams;
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
            var nDaysAgo = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));
            var maxDate = new DateTime(nDaysAgo.Year, nDaysAgo.Month, 1).Subtract(new TimeSpan(1, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            var preparedWhereClause = PrepareWhereClause(query);

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
                AND ( {preparedWhereClause.whereClauseText} )
                GROUP BY citation_yr, citation_mm, citation_mo
            )
            SELECT /*+ RESULT_CACHE */ -- sum previous counts, grouped by series and month
                CAST(citation_yr AS VARCHAR2(4)) AS series,
                citation_mo AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            GROUP BY citation_yr, citation_mm, citation_mo
            ORDER BY citation_yr, citation_mm";

            var dynamicParams = preparedWhereClause.DynamicParams;
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
            DateTime maxDate = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));

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

            var preparedWhereClause = PrepareWhereClause(query);

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
                AND ( {preparedWhereClause.whereClauseText} )
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

            var dynamicParams = preparedWhereClause.DynamicParams;
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


        public ReportOverTime<int> GetCitationCountsByAttribute(int year, string attrName, CitationsOverTimeQuery query)
        {
            // find the date MIN_DAYS_BACK days ago
            DateTime maxDate = DateTime.Now.Subtract(new TimeSpan(MIN_DAYS_BACK, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            var preparedWhereClause = PrepareWhereClause(query);

            string queryText;

            switch (attrName)
            {
                case "violation-type":
                    queryText = $@"WITH
			            grouped_cts AS (
				            SELECT /*+ RESULT_CACHE */
					            nvl(violation_type, 'Unknown') AS category, COUNT(*) AS ct
				            FROM citation
				            WHERE citation_yr = :year
				            AND key_citation_dt < TRUNC(:maxDate + 1)
				            AND ( {preparedWhereClause.whereClauseText} )
				            GROUP BY nvl(violation_type, 'Unknown')
			            )
			            SELECT /*+ RESULT_CACHE */
				            nvl(vv.violation_type, cts.category) AS category,
				            nvl(cts.ct, 0) AS ct
			            FROM v_citation_violation_type vv
			            FULL OUTER JOIN grouped_cts cts
				            ON cts.category = vv.violation_type
			            ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, cts.category";
                    break;
                case "violator-gender":
                    queryText = $@"WITH
                        grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                             CASE
                                WHEN driver_gender_cd = 'M' THEN 'Male'
                                WHEN driver_gender_cd = 'F' THEN 'Female'
                                ELSE 'Unknown'
                              END      AS CATEGORY,
                            COUNT(*) AS ct
                        FROM citation
                        WHERE citation_yr = :year
				            AND key_citation_dt < TRUNC(:maxDate + 1)
				            AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY 
                            CASE
                                WHEN driver_gender_cd = 'M' THEN 'Male' 
                                WHEN driver_gender_cd = 'F' THEN 'Female'
                                ELSE 'Unknown'
                              END
                        )
                        SELECT /*+ RESULT_CACHE */
                        nvl(vv.driver_attr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                        FROM v_driver_gender vv
                        FULL OUTER JOIN grouped_cts cts
                        ON cts.CATEGORY = vv.driver_attr_tx
                        ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, vv.driver_attr_tx";
                    break;
                case "violator-age":
                    queryText = $@"WITH
                        grouped_cts AS (
                            SELECT /*+ RESULT_CACHE */
                                nvl(driver_age_rng, 'Unknown') AS CATEGORY, 
                                COUNT(*) AS ct
                            FROM citation
    	                        WHERE
                                citation_yr = :year AND
			                        key_citation_dt < TRUNC(:maxDate + 1)
				                AND ( {preparedWhereClause.whereClauseText} )
                            GROUP BY nvl(driver_age_rng, 'Unknown')
                            ),
                            grouped_rngs as (
                                SELECT /*+ RESULT_CACHE */
                                    DISTINCT nvl(vv.driver_attr_tx, cts.category) AS category,
                                    nvl(cts.ct, 0) AS ct
                                FROM v_driver_age_rng vv
                                FULL OUTER JOIN grouped_cts cts
                                    ON cts.CATEGORY = vv.driver_attr_tx
                                    )
                                SELECT * FROM grouped_rngs g
                                    ORDER BY 
                                        CASE WHEN CATEGORY = 'Unknown' THEN 3  
                                             WHEN Category = 'Under 15' THEN 1 
                                             ELSE 2 END, category";
                    break;
                default:
                    return null;
            }

            var dynamicParams = preparedWhereClause.DynamicParams;
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
                var seriesName = maxDate.Year.ToString();
                var series = new List<ReportSeries<int>>();
                series.Add(new ReportSeries<int>()
                {
                    name = seriesName,
                    data = results.Select(r => (int)r.CT)
                });
                report.series = series;
            }
            return report;
        }

        private PreparedWhereClause PrepareWhereClause(CitationsOverTimeQuery query)
        {
            // get predicate methods
            var predicateMethods = GetPredicateMethods(query);
            return PrepareWhereClause(predicateMethods);
        }

        private List<Func<(string, object)>> GetPredicateMethods(CitationsOverTimeQuery query)
        {
            Func<(string, object)>[] predicateMethods =
            {
                () => GenerateGeographyPredicate(query.geographyId),
                () => GenerateReportingAgencyPredicate(query.reportingAgencyId),
                () => GenerateCrashInvolvedPredicate(query.crashInvolved),
                () => GenerateClassificationPredicate(query.classification)
            };
            return predicateMethods.ToList();
        }

        private (string whereClause, object parameters) GenerateReportingAgencyPredicate(int? reportingAgencyId)
        {
            // test for valid filter
            if (reportingAgencyId == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:isFhpTroop = 0 AND key_agncy = :reportingAgencyId)
                    OR (:isFhpTroop = 1 AND key_agncy = 1 
                    AND trooper_unit_tx = (SELECT trooper_unit_tx FROM dim_agncy WHERE id = :reportingAgencyId))";

            // define oracle parameters
            var parameters = new
            {
                isFhpTroop = reportingAgencyId > 1 && reportingAgencyId <= 14 ? 1 : 0,
                reportingAgencyId
            };

            return (whereClause, parameters);
        }


        private (string whereClause, object parameters) GenerateClassificationPredicate(string classification)
        {
            // test for valid filter
            if (classification == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"violation_class = '{classification}'";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCrashInvolvedPredicate(bool? crashInvolved)
        {
            // test for valid filter
            if (crashInvolved == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"crash_cd = :crashInvolvedCd";
            // define oracle parameters
            var parameters = new {
                crashInvolvedCd = (bool)crashInvolved ? "Y" : "N"
            };

            return (whereClause, parameters);
        }
    }
}
