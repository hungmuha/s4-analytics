using Dapper;
using Microsoft.Extensions.Options;
using MoreLinq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class CrashReportingRepository: ReportingRepository
    {
        private const int MIN_DAYS_BACK = 15;

        public CrashReportingRepository(IOptions<ServerOptions> serverOptions) : base(serverOptions)
        {
        }

        public ReportOverTime<int> GetCrashCountsByYear(CrashesOverTimeQuery query)
        {
            string[] monthNames = new[] { "","Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };

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
                -- count matching crashes, grouped by year and month
                SELECT /*+ RESULT_CACHE */
                    crash_yr,
                    crash_mm,
                    COUNT(*) ct
                FROM crash_evt
                WHERE key_crash_dt BETWEEN TRUNC(:minDate) AND TRUNC(:maxDate)
                AND ( {preparedWhereClause.whereClauseText} )
                GROUP BY crash_yr, crash_mm
            )
            SELECT /*+ RESULT_CACHE */ -- sum previous counts, grouped by series and year
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

        public ReportOverTime<int> GetCrashCountsByMonth(int year, CrashesOverTimeQuery query)
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
                -- count matching crashes, grouped by year and month
                SELECT /*+ RESULT_CACHE */
                    crash_yr,
                    crash_mm,
                    crash_mo,
                    COUNT(*) ct
                FROM crash_evt
                WHERE crash_yr IN (:year, :year - 1)
                AND key_crash_dt < TRUNC(:maxDate + 1)
                AND ( {preparedWhereClause.whereClauseText} )
                GROUP BY crash_yr, crash_mm, crash_mo
            )
            SELECT /*+ RESULT_CACHE */ -- sum previous counts, grouped by series and month
                CAST(crash_yr AS VARCHAR2(4)) AS series,
                crash_mo AS category,
                SUM(ct) AS ct
            FROM grouped_cts
            GROUP BY crash_yr, crash_mm, crash_mo
            ORDER BY crash_yr, crash_mm";

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

        public ReportOverTime<int?> GetCrashCountsByDay(int year, bool alignByWeek, CrashesOverTimeQuery query)
        {
            // find the date MIN_DAYS_BACK days ago
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
            crash_cts AS (
                SELECT /*+ RESULT_CACHE */
                    key_crash_dt, COUNT(*) AS ct
                FROM crash_evt ce
                WHERE crash_yr BETWEEN :year - 1 AND :year
                AND ( {preparedWhereClause.whereClauseText} )
                GROUP BY key_crash_dt
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
                LEFT OUTER JOIN crash_cts cts
                    ON cts.key_crash_dt = ad.evt_dt
                UNION ALL
                SELECT
                    ad.prev_yr AS yr, ad.seq, ad.prev_yr_dt AS evt_dt, cts.ct
                FROM aligned_dts ad
                LEFT OUTER JOIN crash_cts cts
                    ON cts.key_crash_dt = ad.prev_yr_dt
            ) res
            ORDER BY yr, seq";

            var dynamicParams = preparedWhereClause.DynamicParams;
            dynamicParams.Add(new
            {
                maxDate,
                maxDate.Year
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

        public ReportOverTime<int> GetCrashCountsByAttribute(int year, string attrName, CrashesOverTimeQuery query)
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
                case "weather-condition":
                    queryText = $@"WITH
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            nvl(weather_cond, 'Unknown') AS category, COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY nvl(weather_cond, 'Unknown')
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(vv.crash_attr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM v_crash_weather_cond vv
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = vv.crash_attr_tx
                    ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, vv.crash_attr_cd";
                    break;
                case "light-condition":
                    queryText = $@"WITH
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            nvl(light_cond, 'Unknown') AS category, COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY nvl(light_cond, 'Unknown')
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(vv.crash_attr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM v_crash_light_cond vv
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = vv.crash_attr_tx
                    ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, vv.crash_attr_cd";
                    break;
                case "road-surface-condition":
                    queryText = $@"WITH
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            nvl(rd_surf_cond, 'Unknown') AS category, COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY nvl(rd_surf_cond, 'Unknown')
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(vv.crash_attr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM v_crash_road_surf_cond vv
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = vv.crash_attr_tx
                    ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, vv.crash_attr_cd";
                    break;
                case "crash-type":
                    queryText = $@"WITH
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            crash_type AS category, COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY crash_type
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(vv.crash_attr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM v_crash_type vv
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = vv.crash_attr_tx
                    ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, vv.crash_attr_cd";
                    break;
                case "crash-severity":
                    queryText = $@"WITH
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            crash_sev_dtl AS category,
                            COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY crash_sev_dtl
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(vv.crash_attr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM v_crash_sev_dtl vv
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = vv.crash_attr_tx
                    WHERE vv.crash_attr_cd <> 0
                    ORDER BY vv.crash_attr_cd";
                    break;
                case "first-harmful-event":
                    queryText = $@"WITH
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            nvl(first_he, 'Unknown') AS category,
                            COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY nvl(first_he, 'Unknown')
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(he.harmful_evt_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM dim_harmful_evt he
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = he.harmful_evt_tx
                    ORDER BY CASE WHEN category = 'Unknown' THEN 2 ELSE 1 END, he.harmful_evt_cd";
                    break;
                case "hour-of-day":
                    queryText = $@"WITH
                    hrs AS (
                        SELECT /*+ RESULT_CACHE */
                            COLUMN_VALUE hr,
                            to_char(TO_DATE(COLUMN_VALUE, 'hh24'), 'fmHH AM') hr_tx
                        FROM TABLE(SYS.odcinumberlist(0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23))
                    ),
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            nvl(crash_hh_am, 'Unknown') AS category,
                            COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY nvl(crash_hh_am, 'Unknown')
                    )
                    SELECT /*+ RESULT_CACHE */
                        nvl(hrs.hr_tx, cts.category) AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM hrs
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = hrs.hr_tx
                    ORDER BY CASE WHEN cts.category = 'Unknown' THEN 2 ELSE 1 END, hrs.hr";
                    break;
                case "day-of-week":
                    queryText = $@"WITH
                    days_of_week AS (
                        SELECT /*+ RESULT_CACHE */
                            COLUMN_VALUE day_val,
                            CASE COLUMN_VALUE
                                WHEN 1 THEN 'Sunday'
                                WHEN 2 THEN 'Monday'
                                WHEN 3 THEN 'Tuesday'
                                WHEN 4 THEN 'Wednesday'
                                WHEN 5 THEN 'Thursday'
                                WHEN 6 THEN 'Friday'
                                WHEN 7 THEN 'Saturday'
                            END AS day_tx
                        FROM TABLE(SYS.odcinumberlist(1,2,3,4,5,6,7))
                    ),
                    grouped_cts AS (
                        SELECT /*+ RESULT_CACHE */
                            nvl(crash_day, 'Unknown') AS category,
                            COUNT(*) AS ct
                        FROM crash_evt
                        WHERE crash_yr = :year
                        AND key_crash_dt < TRUNC(:maxDate + 1)
                        AND ( {preparedWhereClause.whereClauseText} )
                        GROUP BY nvl(crash_day, 'Unknown')
                    )
                    SELECT /*+ RESULT_CACHE */
                        ds.day_tx AS category,
                        nvl(cts.ct, 0) AS ct
                    FROM days_of_week ds
                    FULL OUTER JOIN grouped_cts cts
                        ON cts.category = ds.day_tx
                    ORDER BY ds.day_val";
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

        public ReportOverTime<int> GetTimelinessCrashCountsByDay(int year, CrashesOverTimeQuery query)
        {
            var maxDate = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0));

            if (year < maxDate.Year)
            {
                maxDate = new DateTime(year, 12, 31);
            }

            var preparedWhereClause = PrepareWhereClause(query);

            var queryText = $@"SELECT /*+ RESULT_CACHE */
                CASE
                    WHEN hsmv_orig_load_dt_diff BETWEEN 0 AND 7 THEN '0-7 days'
                    WHEN hsmv_orig_load_dt_diff BETWEEN 8 AND 14 THEN '8-14 days'
                    WHEN hsmv_orig_load_dt_diff BETWEEN 15 AND 30 THEN '15-30 days'
                    WHEN hsmv_orig_load_dt_diff > 30 THEN '31+ days'
                END AS series,
                CASE
                    WHEN hsmv_orig_load_dt_diff BETWEEN 0 AND 7 THEN 0
                    WHEN hsmv_orig_load_dt_diff BETWEEN 8 AND 14 THEN 1
                    WHEN hsmv_orig_load_dt_diff BETWEEN 15 AND 30 THEN 2
                    WHEN hsmv_orig_load_dt_diff > 30 THEN 3
                END AS series_sort,
                TO_CHAR(TRUNC(hsmv_orig_load_dt), 'Mon DD') AS category,
                TRUNC(hsmv_orig_load_dt) AS category_sort,
                COUNT(*) AS ct
            FROM crash_evt
                WHERE crash_yr = :year
                AND key_crash_dt < TRUNC(:maxDate + 1)
                AND ( {preparedWhereClause.whereClauseText} )
                AND hsmv_orig_load_dt_diff IS NOT NULL
            GROUP BY
                CASE
                    WHEN hsmv_orig_load_dt_diff BETWEEN 0 AND 7 THEN '0-7 days'
                    WHEN hsmv_orig_load_dt_diff BETWEEN 8 AND 14 THEN '8-14 days'
                    WHEN hsmv_orig_load_dt_diff BETWEEN 15 AND 30 THEN '15-30 days'
                    WHEN hsmv_orig_load_dt_diff > 30 THEN '31+ days'
                END,
                CASE
                    WHEN hsmv_orig_load_dt_diff BETWEEN 0 AND 7 THEN 0
                    WHEN hsmv_orig_load_dt_diff BETWEEN 8 AND 14 THEN 1
                    WHEN hsmv_orig_load_dt_diff BETWEEN 15 AND 30 THEN 2
                    WHEN hsmv_orig_load_dt_diff > 30 THEN 3
                END,
                TRUNC(hsmv_orig_load_dt)
            ORDER BY series_sort, category_sort";

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

        private PreparedWhereClause PrepareWhereClause(CrashesOverTimeQuery query)
        {
            // get predicate methods
            var predicateMethods = GetPredicateMethods(query);
            return PrepareWhereClause(predicateMethods);
        }

        private List<Func<(string, object)>> GetPredicateMethods(CrashesOverTimeQuery query)
        {
            Func<(string, object)>[] predicateMethods =
            {
                () => GenerateGeographyPredicate(query.geographyId),
                () => GenerateReportingAgencyPredicate(query.reportingAgencyId),
                () => GenerateSeverityPredicate(query.severity),
                () => GenerateAlcoholDrugPredicate(query.impairment),
                () => GenerateBikePedPredicate(query.bikePedRelated),
                () => GenerateCmvPredicate(query.cmvRelated),
                () => GenerateCodeablePredicate(query.codeable),
                () => GenerateFormTypePredicate(query.formType)
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
            var whereClause = @"(:isFhpTroop = 0 AND key_rptg_agncy = :reportingAgencyId)
		        OR (:isFhpTroop = 1 AND key_rptg_agncy = 1 AND key_rptg_unit = :reportingAgencyId)";

            // define oracle parameters
            var parameters = new
            {
                isFhpTroop = reportingAgencyId > 1 && reportingAgencyId <= 14 ? 1 : 0,
                reportingAgencyId
            };

            return (whereClause, parameters);
        }


        private (string whereClause, object parameters) GenerateSeverityPredicate(CrashesOverTimeSeverity severity)
        {
            // test for valid filter
            if (severity == null || !(severity.propertyDamageOnly || severity.injury || severity.fatality))
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:pdo = 1 AND key_crash_sev = 30)
                OR (:injury = 1 AND key_crash_sev = 31)
                OR (:fatality = 1 AND key_crash_sev = 32)";

            // define oracle parameters
            var parameters = new {
                pdo = severity.propertyDamageOnly ? 1 : 0,
                injury = severity.injury ? 1 : 0,
                fatality = severity.fatality ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateAlcoholDrugPredicate(CrashesOverTimeImpairment impairment)
        {
            // test for valid filter
            if (impairment == null || !(impairment.drugRelated || impairment.alcoholRelated))
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:drugRelated = 1 AND is_drug_rel = 'Y')
                OR (:alcoholRelated = 1 AND is_alc_rel = 'Y')";

            // define oracle parameters
            var parameters = new {
                drugRelated = impairment.drugRelated ? 1 : 0,
                alcoholRelated = impairment.alcoholRelated ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateBikePedPredicate(CrashesOverTimeBikePedRelated bikePedRelated)
        {
            // test for valid filter
            if (bikePedRelated == null || !(bikePedRelated.bikeRelated || bikePedRelated.pedRelated))
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:bikeRelated = 1 AND (bike_cnt > 0 OR key_first_he = 11))
                OR (:pedRelated = 1 AND (ped_cnt > 0 OR key_first_he = 10))";

            // define oracle parameters
            var parameters = new {
                bikeRelated = bikePedRelated.bikeRelated ? 1 : 0,
                pedRelated = bikePedRelated.pedRelated ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCmvPredicate(bool? cmvRelated)
        {
            // test for valid filter
            if (cmvRelated == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:cmvRelated = 1 AND comm_veh_cnt > 0)
                OR (:cmvRelated = 0 AND comm_veh_cnt = 0)";

            // define oracle parameters
            var parameters = new {
                cmvRelated = cmvRelated == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCodeablePredicate(bool? codeable)
        {
            // test for valid filter
            if (codeable == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"codeable = :codeable";

            // define oracle parameters
            var parameters = new {
                codeable = codeable == true ? "Y" : "N"
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateFormTypePredicate(CrashesOverTimeFormType formTypes)
        {
            // test for valid filter
            if (formTypes == null || !(formTypes.longForm || formTypes.shortForm))
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:longForm = 1 AND form_type_cd = 'L')
                OR (:shortForm = 1 AND form_type_cd = 'S')";

            // define oracle parameters
            var parameters = new {
                longForm = formTypes.longForm ? 1 : 0,
                shortForm = formTypes.shortForm ? 1 : 0
            };

            return (whereClause, parameters);
        }
    }
}
