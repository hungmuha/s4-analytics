using Dapper;
using GeoJSON.Net.Feature;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class CrashRepository
    {
        private readonly string _connStr;
        private readonly string _warehouseSchema;
        private readonly string _spatialSchema;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IList<string> _PSEUDO_EMPTY_STRING_LIST = new List<string>() { "" };
        private readonly IList<int> _PSEUDO_EMPTY_INT_LIST = new List<int>() { -1 };

        public CrashRepository(
            IOptions<ServerOptions> serverOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _connStr = serverOptions.Value.WarehouseConnStr;
            _warehouseSchema = serverOptions.Value.OracleSchemas.Warehouse;
            _spatialSchema = serverOptions.Value.OracleSchemas.Spatial;
            _httpContextAccessor = httpContextAccessor;
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

        public (string, Dictionary<string, object>) CreateQueryTest(CrashQuery query)
        {
            var queryToken = Guid.NewGuid().ToString();
            var preparedQuery = PrepareCrashQuery(query);
            return (preparedQuery.queryText, preparedQuery.queryParameters);
        }

        public string CreateQuery(CrashQuery query) {
            var queryToken = Guid.NewGuid().ToString();
            var preparedQuery = PrepareCrashQuery(query);
            _httpContextAccessor.HttpContext.Session.Set(queryToken, preparedQuery);
            _httpContextAccessor.HttpContext.Session.Set("latest", preparedQuery); // TODO: remove after testing
            return queryToken;
        }

        public bool QueryExists(string queryToken)
        {
            return _httpContextAccessor.HttpContext.Session.Keys.Contains(queryToken);
        }

        public IEnumerable<CrashResult> GetCrashes(string queryToken, int fromIndex, int toIndex) {
            // restrict results to a reasonable number
            var maxRows = 1000;
            toIndex = Math.Min(toIndex, fromIndex + maxRows - 1);

            var preparedQuery = _httpContextAccessor.HttpContext.Session.Get<PreparedQuery>(queryToken);

            var queryText = $@"SELECT *
            FROM (
                SELECT
                fact_crash_evt.hsmv_rpt_nbr AS id,
                fact_crash_evt.key_crash_dt AS crashDate,
                fact_crash_evt.crash_tm AS crashTime,
                fact_crash_evt.hsmv_rpt_nbr AS hsmvReportNumber,
                fact_crash_evt.hsmv_rpt_nbr AS hsmvReportNumberDsp,
                fact_crash_evt.agncy_rpt_nbr AS agencyReportNumber,
                fact_crash_evt.agncy_rpt_nbr AS agencyReportNumberDsp,
                geocode_result.shape_merc.sdo_point.x AS mapPointX,
                geocode_result.shape_merc.sdo_point.y AS mapPointY,
                geocode_result.center_line_x AS centerLineX,
                geocode_result.center_line_y AS centerLineY,
                geocode_result.sym_angle AS symbolAngle,
                geocode_result.crash_seg_id AS crashSegId,
                geocode_result.nearest_intrsect_id AS nearestIntrsectId,
                geocode_result.nearest_intrsect_offset_ft AS nearestIntrsectOffsetFt,
                geocode_result.ref_intrsect_id AS refIntrsectId,
                geocode_result.ref_intrsect_offset_ft AS refIntrsectOffsetFt,
                geocode_result.nearest_intrsect_offset_dir AS nearIntrsectOffsetDir,
                geocode_result.ref_intrsect_offset_dir AS refIntrsectOffsetDir,
                fact_crash_evt.img_ext_tx AS imgExtTx,
                decode(fact_crash_evt.form_type_cd, 'L', 'Long', 'S', 'Short', '') AS formType,
                fact_crash_evt.key_crash_sev AS keyCrashSev,
                fact_crash_evt.key_crash_sev_dtl AS keyCrashSevDtl,
                fact_crash_evt.key_crash_type AS keyCrashType,
                v_crash_sev.crash_attr_tx AS crashSeverity,
                CASE v_crash_sev_dtl.crash_attr_tx WHEN 'No Injuries Coded' THEN 'No Injury' ELSE v_crash_sev_dtl.crash_attr_tx END AS crashSeverityDetailed,
                v_crash_type_simplified.crash_attr_tx AS crashType,
                v_crash_type.crash_attr_tx AS crashTypeDetail,
                v_crash_light_cond.crash_attr_tx AS lightCond,
                v_crash_weather_cond.crash_attr_tx AS weatherCond,
                dim_geography.cnty_nm AS county,
                dim_geography.city_nm AS city,
                geocode_result.st_nm AS streetName,
                geocode_result.intrsect_st_nm AS intersectingStreet,
                fact_crash_evt.is_alc_rel AS isAlcoholRelated,
                fact_crash_evt.is_distracted AS isDistracted,
                fact_crash_evt.is_drug_rel AS isDrugRelated,
                fact_crash_evt.lat AS lat,
                fact_crash_evt.lng AS lng,
                fact_crash_evt.offset_dir AS offsetDir,
                fact_crash_evt.offset_ft AS offsetFt,
                fact_crash_evt.veh_cnt AS vehicleCount,
                fact_crash_evt.nm_cnt AS nonmotoristCount,
                fact_crash_evt.fatality_cnt AS fatalityCount,
                fact_crash_evt.inj_cnt AS injuryCount,
                fact_crash_evt.tot_dmg_amt AS totDmgAmt,
                dim_agncy.agncy_short_nm AS agncyNm,
                dim_agncy.ID AS agncyId,
                geocode_result.cnty_cd AS cntyCd,
                geocode_result.city_cd AS cityCd,
                fact_crash_evt.crash_type_dir_tx AS crashTypeDir,
                v_crash_road_surf_cond.crash_attr_tx AS crashRoadSurfCond,
                dim_harmful_evt.harmful_evt_tx AS firstHarmfulEvent,
                CASE
                  WHEN v_bike_ped_crash_type.bike_or_ped IS NOT NULL THEN v_bike_ped_crash_type.bike_or_ped
                  WHEN (fact_crash_evt.ped_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedestrian') AND (fact_crash_evt.bike_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedalcycle') THEN '?'
                  WHEN fact_crash_evt.ped_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedestrian' THEN 'P'
                  WHEN fact_crash_evt.bike_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedalcycle' THEN 'B'
                END AS bikeOrPed,
                v_bike_ped_crash_type.crash_type_nm AS bikePedCrashTypeName,
                fact_crash_evt.bike_cnt AS bikeCount,
                fact_crash_evt.ped_cnt AS pedCount,
                fact_crash_evt.inj_none_cnt as injuryNoneCount,
                fact_crash_evt.inj_possible_cnt AS injuryPossibleCount,
                fact_crash_evt.inj_non_incapacitating_cnt AS injuryNonIncapacitatingCount,
                fact_crash_evt.inj_incapacitating_cnt AS injuryIncapacitatingCount,
                fact_crash_evt.inj_fatal_30_cnt as injuryFatal30Count,
                fact_crash_evt.inj_fatal_non_traffic_cnt AS injuryFatalNonTrafficCount,
                ROWNUM AS rnum
                FROM {_warehouseSchema}.fact_crash_evt
                INNER JOIN ({preparedQuery.queryText}) prepared_query
                  ON prepared_query.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
                INNER JOIN {_spatialSchema}.geocode_result geocode_result
                  ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
                LEFT JOIN {_warehouseSchema}.dim_agncy
                  ON fact_crash_evt.key_rptg_agncy = dim_agncy.ID
                LEFT JOIN {_warehouseSchema}.dim_geography
                  ON fact_crash_evt.key_geography = dim_geography.ID
                LEFT JOIN {_warehouseSchema}.v_crash_sev
                  ON fact_crash_evt.key_crash_sev = v_crash_sev.ID
                LEFT JOIN {_warehouseSchema}.v_crash_sev_dtl
                  ON fact_crash_evt.key_crash_sev_dtl = v_crash_sev_dtl.ID
                LEFT JOIN {_warehouseSchema}.v_crash_weather_cond
                  ON fact_crash_evt.key_weather_cond = v_crash_weather_cond.ID
                LEFT JOIN {_warehouseSchema}.v_crash_light_cond
                  ON fact_crash_evt.key_light_cond = v_crash_light_cond.ID
                LEFT JOIN {_warehouseSchema}.v_crash_type_simplified
                  ON fact_crash_evt.key_crash_type = v_crash_type_simplified.ID
                LEFT JOIN {_warehouseSchema}.v_crash_type
                  ON fact_crash_evt.key_crash_type = v_crash_type.ID
                LEFT JOIN {_warehouseSchema}.v_crash_road_surf_cond
                  ON fact_crash_evt.key_rd_surf_cond = v_crash_road_surf_cond.ID
                LEFT JOIN {_warehouseSchema}.dim_harmful_evt
                  ON fact_crash_evt.key_1st_he = dim_harmful_evt.ID
                LEFT JOIN {_warehouseSchema}.v_bike_ped_crash_type
                  ON fact_crash_evt.key_bike_ped_crash_type = v_bike_ped_crash_type.crash_type_id
                WHERE ROWNUM <= :toIndex + 1
            )
            WHERE rnum >= :fromIndex + 1";

            var dynamicParams = preparedQuery.DynamicParams;
            dynamicParams.Add(new { fromIndex, toIndex });

            using (var conn = new OracleConnection(_connStr))
            {
                var crashResults = conn.Query<CrashResult>(queryText, dynamicParams);
                return crashResults;
            }
        }

        public IEnumerable<AttributeSummary> GetCrashSeveritySummary(string queryToken)
        {
            // TODO: move to a new repository for summary results?
            // TODO: parameterize the specific attribute(s) to summarize
            // TODO: store natural sort order in database

            var preparedQuery = _httpContextAccessor.HttpContext.Session.Get<PreparedQuery>(queryToken);

            var queryText = $@"SELECT attribute, COUNT(*) AS count
                FROM (
                  SELECT
                  CASE v_crash_sev_dtl.ID WHEN 220 THEN 221 ELSE v_crash_sev_dtl.ID END AS sort_order,
                  CASE v_crash_sev_dtl.crash_attr_tx WHEN 'No Injuries Coded' THEN 'No Injury' ELSE v_crash_sev_dtl.crash_attr_tx END AS attribute
                  FROM {_warehouseSchema}.fact_crash_evt
                  INNER JOIN ({preparedQuery.queryText}) prepared_query
                    ON prepared_query.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
                  LEFT JOIN {_warehouseSchema}.v_crash_sev_dtl
                    ON fact_crash_evt.key_crash_sev_dtl = v_crash_sev_dtl.ID
                )
                GROUP BY attribute, sort_order
                ORDER BY sort_order";

            using (var conn = new OracleConnection(_connStr))
            {
                var summary = conn.Query<AttributeSummary>(queryText, preparedQuery.DynamicParams);
                return summary;
            }
        }

        public EventFeatureSet GetCrashFeatureCollection(string queryToken, Extent mapExtent)
        {
            const int maxPoints = 100000;

            var preparedQuery = _httpContextAccessor.HttpContext.Session.Get<PreparedQuery>(queryToken);

            var dynamicParams = preparedQuery.DynamicParams;
            dynamicParams.Add(new
            {
                mapExtentMinX = mapExtent.minX,
                mapExtentMinY = mapExtent.minY,
                mapExtentMaxX = mapExtent.maxX,
                mapExtentMaxY = mapExtent.maxY
            });

            var countQueryText = $@"SELECT COUNT(*)
                FROM {_warehouseSchema}.fact_crash_evt
                INNER JOIN ({preparedQuery.queryText}) prepared_query
                  ON prepared_query.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
                INNER JOIN {_spatialSchema}.geocode_result geocode_result
                  ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
                WHERE geocode_result.shape_merc.sdo_point.x BETWEEN :mapExtentMinX AND :mapExtentMaxX
                  AND geocode_result.shape_merc.sdo_point.y BETWEEN :mapExtentMinY AND :mapExtentMaxY";

            int eventCount;

            using (var conn = new OracleConnection(_connStr))
            {
                eventCount = conn.QuerySingleOrDefault<int>(countQueryText, dynamicParams);
            }

            string queryText;
            var useSample = eventCount > maxPoints;
            if (useSample)
            {
                var samplePercentage = 100.0 * maxPoints / eventCount;
                queryText = $@"WITH sample_evts AS (
                  SELECT hsmv_rpt_nbr
                  FROM {_warehouseSchema}.fact_crash_evt
                  SAMPLE({samplePercentage})
                )
                SELECT
                  NULL AS eventId, -- do not retrieve ids for sample
                  geocode_result.shape_merc.sdo_point.x AS x,
                  geocode_result.shape_merc.sdo_point.y AS y
                FROM {_warehouseSchema}.fact_crash_evt
                INNER JOIN sample_evts
                  ON sample_evts.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
                INNER JOIN ({preparedQuery.queryText}) prepared_query
                    ON prepared_query.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
                INNER JOIN {_spatialSchema}.geocode_result geocode_result
                    ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
                WHERE geocode_result.shape_merc.sdo_point.x BETWEEN :mapExtentMinX AND :mapExtentMaxX
                AND geocode_result.shape_merc.sdo_point.y BETWEEN :mapExtentMinY AND :mapExtentMaxY";
            }
            else
            {
                queryText = $@"SELECT
                  fact_crash_evt.hsmv_rpt_nbr AS eventId,
                  geocode_result.shape_merc.sdo_point.x AS x,
                  geocode_result.shape_merc.sdo_point.y AS y
                FROM {_warehouseSchema}.fact_crash_evt
                INNER JOIN ({preparedQuery.queryText}) prepared_query
                    ON prepared_query.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
                INNER JOIN {_spatialSchema}.geocode_result geocode_result
                    ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
                WHERE geocode_result.shape_merc.sdo_point.x BETWEEN :mapExtentMinX AND :mapExtentMaxX
                AND geocode_result.shape_merc.sdo_point.y BETWEEN :mapExtentMinY AND :mapExtentMaxY";
            }

            List<EventPoint> eventPoints;
            using (var conn = new OracleConnection(_connStr))
            {
                eventPoints = conn.Query<EventPoint>(queryText, dynamicParams)
                    .ToList();
            }
            var features = eventPoints
                    .Select(eventPoint => eventPoint.ToFeature())
                    .ToList();

            var eventFeatureColl = new EventFeatureSet()
            {
                eventType = "crash",
                isSample = useSample,
                eventCount = eventCount,
                sampleSize = useSample ? features.Count : 0,
                sampleMultiplier = useSample ? (double)eventCount / features.Count : 0.0,
                featureCollection = new FeatureCollection(features)
            };

            return eventFeatureColl;
        }

        private PreparedQuery PrepareCrashQuery(CrashQuery query)
        {
            var queryText = $@"SELECT /*+ RESULT_CACHE */
                  fact_crash_evt.hsmv_rpt_nbr
                FROM {_warehouseSchema}.fact_crash_evt
                INNER JOIN {_spatialSchema}.geocode_result geocode_result
                  ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
                LEFT JOIN {_warehouseSchema}.dim_harmful_evt
                  ON fact_crash_evt.key_1st_he = dim_harmful_evt.ID";

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

            // join where clauses; append to insert statement
            queryText += "\r\nWHERE (" + string.Join(")\r\nAND (", whereClauses) + ")";

            return new PreparedQuery(queryText, queryParameters);
        }

        private List<Func<(string, object)>> GetPredicateMethods(CrashQuery query)
        {
            Func<(string, object)>[] predicateMethods =
            {
                () => GenerateDateRangePredicate(query.dateRange),
                () => GenerateDayOfWeekPredicate(query.dayOfWeek),
                () => GenerateTimeRangePredicate(query.timeRange),
                () => GenerateDotDistrictPredicate(query.dotDistrict),
                () => GenerateMpoTpoPredicate(query.mpoTpo),
                () => GenerateCountyPredicate(query.county),
                () => GenerateCityPredicate(query.city),
                () => GenerateCustomAreaPredicate(query.customArea),
                () => GenerateCustomExtentPredicate(query.customExtent),
                () => GenerateIntersectionPredicate(query.intersection),
                () => GenerateStreetPredicate(query.street),
                () => GenerateCustomNetworkPredicate(query.customNetwork),
                () => GeneratePublicRoadOnlyPredicate(query.publicRoadOnly),
                () => GenerateFormTypePredicate(query.formType),
                () => GenerateCodeableOnlyPredicate(query.codeableOnly),
                () => GenerateReportingAgencyPredicate(query.reportingAgency),
                () => GenerateDriverGenderPredicate(query.driverGender),
                () => GenerateDriverAgePredicate(query.driverAgeRange),
                () => GeneratePedestrianAgePredicate(query.pedestrianAgeRange),
                () => GenerateCyclistAgePredicate(query.cyclistAgeRange),
                () => GenerateNonAutoModeOfTravelPredicate(query.nonAutoModesOfTravel),
                () => GenerateSourceOfTransportPredicate(query.sourcesOfTransport),
                () => GenerateBehavioralFactorPredicate(query.behavioralFactors),
                () => GenerateCommonViolationPredicate(query.commonViolations),
                () => GenerateVehicleTypePredicate(query.vehicleType),
                () => GenerateCrashTypeSimplePredicate(query.crashTypeSimple),
                () => GenerateCrashTypeDetailedPredicate(query.crashTypeDetailed),
                () => GenerateBikePedCrashTypePredicate(query.bikePedCrashType),
                () => GenerateCmvConfigurationPredicate(query.cmvConfiguration),
                () => GenerateEnvironmentalCircumstancePredicate(query.environmentalCircumstance),
                () => GenerateRoadCircumstancePredicate(query.roadCircumstance),
                () => GenerateFirstHarmfulEventPredicate(query.firstHarmfulEvent),
                () => GenerateLightConditionPredicate(query.lightCondition),
                () => GenerateRoadSystemIdentifierPredicate(query.roadSystemIdentifier),
                () => GenerateWeatherConditionPredicate(query.weatherCondition),
                () => GenerateLaneDeparturePredicate(query.laneDepartures),
                () => GenerateOtherCircumstancePredicate(query.otherCircumstances)
            };
            return predicateMethods.ToList();
        }

        private (string whereClause, object parameters) GenerateDateRangePredicate(DateRange dateRange)
        {
            // test for valid filter
            if (dateRange == null) {
                return (null, null);
            }

            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_CRASH_DT BETWEEN :startDate AND :endDate";

            // define oracle parameters
            var parameters = new {
                startDate = new DateTime(
                    dateRange.startDate.Year, dateRange.startDate.Month, dateRange.startDate.Day, 0, 0, 0, 0),
                endDate = new DateTime(
                    dateRange.endDate.Year, dateRange.endDate.Month, dateRange.endDate.Day, 23, 59, 59, 999)
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDayOfWeekPredicate(IList<int> dayOfWeek)
        {
            // test for valid filter
            if (dayOfWeek == null || dayOfWeek.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "TO_CHAR(FACT_CRASH_EVT.KEY_CRASH_DT, 'D') IN :dayOfWeek";

            // define oracle parameters
            var parameters = new { dayOfWeek };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateTimeRangePredicate(TimeRange timeRange)
        {
            // test for valid filter
            if (timeRange == null) {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(
                :startTime <= :endTime -- same day
                AND CAST(TO_CHAR(CRASH_TM, 'HH24MI') AS INTEGER) BETWEEN :startTime AND :endTime
            ) OR (
                :startTime > :endTime -- crosses midnight boundary
                AND (
                  CAST(TO_CHAR(CRASH_TM, 'HH24MI') AS INTEGER) >= :startTime
                  OR CAST(TO_CHAR(CRASH_TM, 'HH24MI') AS INTEGER) <= :endTime
                )
            )";

            // define oracle parameters
            var parameters = new {
                startTime = timeRange.startTime.Hour*100 + timeRange.startTime.Minute,
                endTime = timeRange.endTime.Hour*100 + timeRange.endTime.Minute
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDotDistrictPredicate(IList<int> dotDistrict)
        {
            // TODO: tag crashes with dot district in oracle

            // test for valid filter
            if (dotDistrict == null || dotDistrict.Count == 0)
            {
                return (null, null);
            }

            // map dot district codes to county codes
            var countyCodeMap = new Dictionary<int, int[]>()
            {
                { 1, new[] { 5, 15, 16, 18, 27, 30, 34, 49, 53, 57, 60, 64 } },
                { 2, new[] { 2, 11, 20, 22, 29, 31, 35, 37, 39, 41, 45, 48, 52, 54, 55, 56, 62, 63 } },
                { 3, new[] { 9, 13, 21, 23, 25, 33, 36, 43, 46, 50, 51, 58, 59, 65, 66, 67 } },
                { 4, new[] { 6, 10, 24, 32, 42 } },
                { 5, new[] { 7, 8, 12, 14, 17, 19, 26, 44, 61 } },
                { 6, new[] { 1, 38 } },
                { 7, new[] { 3, 4, 28, 40, 47 } }
            };
            var dotDistrictCountyCodes = new List<int>();
            foreach (var districtNumber in dotDistrict)
            {
                dotDistrictCountyCodes.AddRange(countyCodeMap[districtNumber]);
            }

            // define where clause
            var whereClause = @"FLOOR(FACT_CRASH_EVT.KEY_GEOGRAPHY / 100) IN :dotDistrictCountyCodes
                OR FLOOR(GEOCODE_RESULT.KEY_GEOGRAPHY / 100) IN :dotDistrictCountyCodes";

            // define oracle parameters
            var parameters = new { dotDistrictCountyCodes };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateMpoTpoPredicate(IList<int> mpoTpo)
        {
            // TODO: tag crashes with mpo/tpo in oracle

            // test for valid filter
            if (mpoTpo == null || mpoTpo.Count == 0)
            {
                return (null, null);
            }

            // extract partial county mpo/tpo ids
            var partialCountyMpos = new[]
            {
                6,  // Florida-Alabama TPO
                7,  // Gainesville MTPO
                10, // Indian River County MPO
                18  // Okaloosa-Walton TPO
            };
            var partialCountyMpoIds = partialCountyMpos.Where(mpoId => mpoTpo.Contains<int>(mpoId)).AsList();

            // map mpo/tpo ids to county codes
            var countyCodeMap = new Dictionary<int, int[]>
            {
                { 1,  new[] { 23 } },                // Bay County MPO
                { 2,  new[] { 10 } },                // Broward MPO
                { 3,  new[] { 13, 21, 46, 65 } },    // Capital Region TPA
                { 4,  new[] { 53 } },                // Charlotte County-Punta Gorda MPO
                { 5,  new[] { 64 } },                // Collier County MPO
                { 6,  new[] { 9, 33 } },             // Florida-Alabama TPO
                { 7,  new[] { 11 } },                // Gainesville MTPO
                { 8,  new[] { 40 } },                // Hernando County MPO
                { 9,  new[] { 3 } },                 // Hillsborough County MPO
                { 10, new[] { 32 } },                // Indian River County MPO
                { 11, new[] { 12, 44 } },            // Lake-Sumter MPO
                { 12, new[] { 18 } },                // Lee County MPO
                { 13, new[] { 42 } },                // Martin County MPO
                { 14, new[] { 7, 17, 26 } },         // Metroplan Orlando
                { 15, new[] { 1 } },                 // Miami-Dade Urbanized Area MPO
                { 16, new[] { 2, 20, 41, 48 } },     // North Florida TPO
                { 17, new[] { 14 } },                // Ocala-Marion County TPO
                { 18, new[] { 36, 43 } },            // Okaloosa-Walton TPO
                { 19, new[] { 6 } },                 // Palm Beach County MPO
                { 20, new[] { 28 } },                // Pasco County MPO
                { 21, new[] { 4 } },                 // Pinellas County MPO
                { 22, new[] { 5 } },                 // Polk TPO
                { 23, new[] { 15, 16 } },            // Sarasota-Manatee MPO
                { 24, new[] { 19 } },                // Space Coast TPO
                { 25, new[] { 24 } },                // St Lucie TPO
                { 26, new[] { 8 } }                  // Volusia County MPO
            };
            var mpoCountyCodes = new List<int>();
            foreach (var mpoId in mpoTpo)
            {
                mpoCountyCodes.AddRange(countyCodeMap[mpoId]);
            }

            // define where clause
            var whereClause = $@"FLOOR(FACT_CRASH_EVT.KEY_GEOGRAPHY / 100) IN :mpoCountyCodes
                OR FLOOR(GEOCODE_RESULT.KEY_GEOGRAPHY / 100) IN :mpoCountyCodes
                OR EXISTS (
                    SELECT NULL FROM {_spatialSchema}.ST_EXT
                    WHERE LINK_ID = GEOCODE_RESULT.CRASH_SEG_ID
                    AND MPO_BND_ID IN :partialCountyMpoIds
                )";

            // define oracle parameters
            var parameters = new {
                mpoCountyCodes = mpoCountyCodes.Any() ? mpoCountyCodes : _PSEUDO_EMPTY_INT_LIST,
                partialCountyMpoIds = partialCountyMpoIds.Any() ? partialCountyMpoIds : _PSEUDO_EMPTY_INT_LIST
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCountyPredicate(IList<int> county)
        {
            // TODO: join to dim_geography rather than divide by 100 in where clause

            // test for valid filter
            if (county == null || county.Count == 0)
            {
                return (null, null);
            }

            // extract full county codes
            var fullCountyCodes = county.Where(countyCode => countyCode < 100).ToList();

            // extract unincorporated county codes
            var unincorporatedCountyCodes = county.Where(countyCode => countyCode >= 100).ToList();

            // define where clause
            var whereClause = @"FLOOR(FACT_CRASH_EVT.KEY_GEOGRAPHY/100) IN :fullCountyCodes
                OR FLOOR(GEOCODE_RESULT.KEY_GEOGRAPHY/100) IN :fullCountyCodes
                OR FACT_CRASH_EVT.KEY_GEOGRAPHY IN :unincorporatedCountyCodes
                OR GEOCODE_RESULT.KEY_GEOGRAPHY IN :unincorporatedCountyCodes";

            // define oracle parameters
            var parameters = new {
                fullCountyCodes = fullCountyCodes.Any() ? fullCountyCodes : _PSEUDO_EMPTY_INT_LIST,
                unincorporatedCountyCodes = unincorporatedCountyCodes.Any() ? unincorporatedCountyCodes : _PSEUDO_EMPTY_INT_LIST
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCityPredicate(IList<int> city)
        {
            // test for valid filter
            if (city == null || city.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_GEOGRAPHY IN :cityCodes OR GEOCODE_RESULT.KEY_GEOGRAPHY IN :cityCodes";

            // define oracle parameters
            var parameters = new {
                cityCodes = city
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCustomAreaPredicate(IList<Coordinates> customArea)
        {
            // test for valid filter
            if (customArea == null || customArea.Count == 0)
            {
                return (null, null);
            }

            var customAreaMinX = customArea.Min(coords => coords.x);
            var customAreaMinY = customArea.Min(coords => coords.y);
            var customAreaMaxX = customArea.Max(coords => coords.x);
            var customAreaMaxY = customArea.Max(coords => coords.y);
            var coordsAsText = customArea.Select(coords => $"{coords.x} {coords.y}");
            var customAreaPolygon = $"POLYGON (({string.Join(", ", coordsAsText)}))";

            // define where clause
            var whereClause = @"geocode_result.shape_merc.sdo_point.x BETWEEN :customAreaMinX AND :customAreaMaxX
                geocode_result.shape_merc.sdo_point.y BETWEEN :customAreaMinY AND :customAreaMaxY
                AND sdo_relate(geocode_result.shape_merc, sdo_geometry(:customAreaPolygon, :customAreaSrid)) = 'TRUE'";

            // define oracle parameters
            var parameters = new {
                customAreaMinX, customAreaMinY, customAreaMaxX, customAreaMaxY,
                customAreaPolygon, customAreaSrid = 3087 // TODO: parameterize srid
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCustomExtentPredicate(Extent customExtent)
        {
            // test for valid filter
            if (customExtent == null || !customExtent.IsValid)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"geocode_result.shape_merc.sdo_point.x BETWEEN :customExtentMinX AND :customExtentMaxX
                AND geocode_result.shape_merc.sdo_point.y BETWEEN :customExtentMinY AND :customExtentMaxY";

            // define oracle parameters
            var parameters = new {
                customExtentMinX = customExtent.minX,
                customExtentMinY = customExtent.minY,
                customExtentMaxX = customExtent.maxX,
                customExtentMaxY = customExtent.maxY
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateIntersectionPredicate(IntersectionParameters intersection)
        {
            // test for valid filter
            if (intersection == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_spatialSchema}.GEOCODE_RESULT
                WHERE GEOCODE_RESULT.HSMV_RPT_NBR = FACT_CRASH_EVT.HSMV_RPT_NBR
                AND GEOCODE_RESULT.NEAREST_INTRSECT_ID = :intersectionId
                AND GEOCODE_RESULT.NEAREST_INTRSECT_OFFSET_FT <= :intersectionOffsetFeet
            )
            AND (:matchAnyIntersectionOffsetDir = 1 OR FACT_CRASH_EVT.OFFSET_DIR IN :intersectionOffsetDirs)";

            // define oracle parameters
            var anyOffsetDir = intersection.offsetDirection == null || intersection.offsetDirection.Count == 0;
            var parameters = new {
                intersection.intersectionId,
                intersectionOffsetFeet = intersection.offsetInFeet,
                intersectionOffsetDirs = anyOffsetDir ? _PSEUDO_EMPTY_STRING_LIST : intersection.offsetDirection,
                matchAnyIntersectionOffsetDir = anyOffsetDir ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateStreetPredicate(StreetParameters street)
        {
            // TODO: refactor query to use WHERE EXISTS instead of IN

            // test for valid filter
            if (street == null || street.linkIds == null || street.linkIds.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"GEOCODE_RESULT.CRASH_SEG_ID IN :streetLinkIds
                OR (
                  :matchCrossStreets = 1
                  AND GEOCODE_RESULT.NEAREST_INTRSECT_OFFSET_FT <= 100
                  AND GEOCODE_RESULT.NEAREST_INTRSECT_ID IN (
                    SELECT DISTINCT INTRSECT_NODE.INTERSECTION_ID
                    FROM {_spatialSchema}.INTRSECT_NODE
                    WHERE INTRSECT_NODE.NODE_ID IN (
                        SELECT ST.REF_IN_ID
                        FROM {_spatialSchema}.ST
                        WHERE ST.LINK_ID IN :streetLinkIds
                        UNION
                        SELECT ST.NREF_IN_ID
                        FROM {_spatialSchema}.ST
                        WHERE ST.LINK_ID IN :streetLinkIds
                    )
                  )
                )";

            // define oracle parameters
            var parameters = new {
                streetLinkIds = street.linkIds,
                matchCrossStreets = street.includeCrossStreets ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCustomNetworkPredicate(IList<int> customNetwork)
        {
            // test for valid filter
            if (customNetwork == null || customNetwork.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "GEOCODE_RESULT.CRASH_SEG_ID IN :customNetworkLinkIds";

            // define oracle parameters
            var parameters = new {
                customNetworkLinkIds = customNetwork
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GeneratePublicRoadOnlyPredicate(bool? publicRoadOnly)
        {
            // test for valid filter
            if (publicRoadOnly != true)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_RD_SYS_ID IN (140,141,142,143,144,145)";

            return (whereClause, null);
        }

        private (string whereClause, object parameters) GenerateFormTypePredicate(IList<string> formType)
        {
            // test for valid filter
            if (formType == null || formType.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "FACT_CRASH_EVT.FORM_TYPE_CD IN :formType";

            // define oracle parameters
            var parameters = new { formType };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCodeableOnlyPredicate(bool? codeableOnly)
        {
            // test for valid filter
            if (codeableOnly != true)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "FACT_CRASH_EVT.CODEABLE = 'T'";

            return (whereClause, null);
        }

        private (string whereClause, object parameters) GenerateReportingAgencyPredicate(IList<int> reportingAgency)
        {
            // TODO: move fhp troop map to oracle

            // test for valid filter
            if (reportingAgency == null || reportingAgency.Count == 0)
            {
                return (null, null);
            }

            var fhpTroopMap = new Dictionary<int, string>()
            {
                {2, "A"},
                {3, "B"},
                {4, "C"},
                {5, "D"},
                {6, "E"},
                {7, "F"},
                {8, "G"},
                {9, "H"},
                {12, "I"},
                {13, "J"},
                {10, "K"},
                {11, "L"},
                {14, "Q"}
            };
            var agencyIds = reportingAgency
                .Where(agencyId => agencyId == 1 || agencyId > 14)
                .ToList();
            var fhpTroops = reportingAgency
                .Where(agencyId => agencyId > 1 && agencyId <= 14)
                .Select(agencyId => fhpTroopMap[agencyId])
                .ToList();

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_RPTG_AGNCY IN :agencyIds
                OR (FACT_CRASH_EVT.KEY_RPTG_AGNCY = 1 AND FACT_CRASH_EVT.KEY_RPTG_UNIT IN :fhpTroops)";

            // define oracle parameters
            var parameters = new {
                agencyIds = agencyIds.Any() ? agencyIds : _PSEUDO_EMPTY_INT_LIST,
                fhpTroops = fhpTroops.Any() ? fhpTroops : _PSEUDO_EMPTY_STRING_LIST
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDriverGenderPredicate(IList<int> driverGender)
        {
            // test for valid filter
            if (driverGender == null || driverGender.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.FACT_DRIVER
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_DRIVER.HSMV_RPT_NBR
                AND FACT_DRIVER.KEY_GENDER IN :driverGender
            )";

            // define oracle parameters
            var parameters = new { driverGender };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDriverAgePredicate(IList<string> driverAgeRange)
        {
            // TODO: give age ranges a database identifier

            // test for valid filter
            if (driverAgeRange == null || driverAgeRange.Count == 0)
            {
                return (null, null);
            }

            var unknownAge = driverAgeRange.Where(ageRange => ageRange == "Unknown").Any();
            var ageRanges = driverAgeRange.Where(ageRange => ageRange != "Unknown").ToList();

            // define where clause
            var whereClause = $@"(:matchUnknownDriverAge = 1
                AND NOT EXISTS (
                    SELECT NULL FROM {_warehouseSchema}.FACT_DRIVER
                    WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_DRIVER.HSMV_RPT_NBR
                )
            ) OR EXISTS (
                SELECT NULL FROM {_warehouseSchema}.FACT_DRIVER
                LEFT OUTER JOIN {_warehouseSchema}.V_DRIVER_AGE_RNG
                    ON FACT_DRIVER.KEY_AGE_RNG = V_DRIVER_AGE_RNG.ID
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_DRIVER.HSMV_RPT_NBR
                AND (
                    V_DRIVER_AGE_RNG.DRIVER_ATTR_TX IN :driverAgeRanges
                    OR (:matchUnknownDriverAge = 1 AND V_DRIVER_AGE_RNG.DRIVER_ATTR_TX IS NULL)
                )
            )";

            // define oracle parameters
            var parameters = new {
                driverAgeRanges = ageRanges.Any() ? ageRanges : _PSEUDO_EMPTY_STRING_LIST,
                matchUnknownDriverAge = unknownAge ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GeneratePedestrianAgePredicate(IList<string> pedestrianAgeRange)
        {
            // test for valid filter
            if (pedestrianAgeRange == null || pedestrianAgeRange.Count == 0)
            {
                return (null, null);
            }

            var unknownAge = pedestrianAgeRange.Where(ageRange => ageRange == "Unknown").Any();
            var ageRanges = pedestrianAgeRange.Where(ageRange => ageRange != "Unknown").ToList();

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.FACT_NON_MOTORIST
                LEFT OUTER JOIN {_warehouseSchema}.V_NM_AGE_RNG
                    ON FACT_NON_MOTORIST.KEY_AGE_RNG = V_NM_AGE_RNG.ID
                WHERE FACT_NON_MOTORIST.KEY_DESC IN (230, 231)
                AND FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_NON_MOTORIST.HSMV_RPT_NBR
                AND (
                    V_NM_AGE_RNG.NM_ATTR_TX IN :pedestrianAgeRanges
                    OR (:matchUnknownPedestrianAge = 1 AND V_NM_AGE_RNG.NM_ATTR_TX IS NULL)
                )
            )";

            // define oracle parameters
            var parameters = new {
                pedestrianAgeRanges = ageRanges.Any() ? ageRanges : _PSEUDO_EMPTY_STRING_LIST,
                matchUnknownPedestrianAge = unknownAge ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCyclistAgePredicate(IList<string> cyclistAgeRange)
        {
            // test for valid filter
            if (cyclistAgeRange == null || cyclistAgeRange.Count == 0)
            {
                return (null, null);
            }

            var unknownAge = cyclistAgeRange.Where(ageRange => ageRange == "Unknown").Any();
            var ageRanges = cyclistAgeRange.Where(ageRange => ageRange != "Unknown").ToList();

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.FACT_NON_MOTORIST
                LEFT OUTER JOIN {_warehouseSchema}.V_NM_AGE_RNG
                    ON FACT_NON_MOTORIST.KEY_AGE_RNG = V_NM_AGE_RNG.ID
                WHERE FACT_NON_MOTORIST.KEY_DESC IN (232, 233)
                AND FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_NON_MOTORIST.HSMV_RPT_NBR
                AND (
                  V_NM_AGE_RNG.NM_ATTR_TX IN :cyclistAgeRanges
                  OR (:matchUnknownCyclistAge = 1 AND V_NM_AGE_RNG.NM_ATTR_TX IS NULL)
                )
            )";

            // define oracle parameters
            var parameters = new {
                cyclistAgeRanges = ageRanges.Any() ? ageRanges : _PSEUDO_EMPTY_STRING_LIST,
                matchUnknownCyclistAge = unknownAge ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateNonAutoModeOfTravelPredicate(NonAutoModesOfTravel nonAutoModesOfTravel)
        {
            // test for valid filter
            if (nonAutoModesOfTravel == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:matchNonAutoModePed = 1 AND FACT_CRASH_EVT.PED_CNT > 0)
                OR (:matchNonAutoModeBike = 1 AND FACT_CRASH_EVT.BIKE_CNT > 0)
                OR (:matchNonAutoModeMoped = 1 AND FACT_CRASH_EVT.MOPED_CNT > 0)
                OR (:matchNonAutoModeMotorcycle = 1 AND FACT_CRASH_EVT.MOTORCYCLE_CNT > 0)";

            // define oracle parameters
            var parameters = new {
                matchNonAutoModePed = nonAutoModesOfTravel.pedestrian == true ? 1 : 0,
                matchNonAutoModeBike = nonAutoModesOfTravel.bicyclist == true ? 1 : 0,
                matchNonAutoModeMoped = nonAutoModesOfTravel.moped == true ? 1 : 0,
                matchNonAutoModeMotorcycle = nonAutoModesOfTravel.motorcycle == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateSourceOfTransportPredicate(SourcesOfTransport sourcesOfTransport)
        {
            // test for valid filter
            if (sourcesOfTransport == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:matchTransportEms = 1 AND FACT_CRASH_EVT.TRANS_BY_EMS_CNT > 0)
                OR (:matchTransportLawEnforcement = 1 AND FACT_CRASH_EVT.TRANS_BY_LE_CNT > 0)
                OR (:matchTransportOther = 1 AND FACT_CRASH_EVT.TRANS_BY_OTH_CNT > 0)";

            // define oracle parameters
            var parameters = new {
                matchTransportEms = sourcesOfTransport.ems == true ? 1 : 0,
                matchTransportLawEnforcement = sourcesOfTransport.lawEnforcement == true ? 1 : 0,
                matchTransportOther = sourcesOfTransport.other == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateBehavioralFactorPredicate(BehavioralFactors behavioralFactors)
        {
            // test for valid filter
            if (behavioralFactors == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:matchBehavioralAlcohol = 1 AND FACT_CRASH_EVT.IS_ALC_REL = '1')
                OR (:matchBehavioralDrugs = 1 AND FACT_CRASH_EVT.IS_DRUG_REL = '1')
                OR (:matchBehavioralDistraction = 1 AND FACT_CRASH_EVT.IS_DISTRACTED = '1')
                OR (:matchBehavioralAggressive = 1 AND FACT_CRASH_EVT.IS_AGGRESSIVE = '1')";

            // define oracle parameters
            var parameters = new {
                matchBehavioralAlcohol = behavioralFactors.alcohol == true ? 1 : 0,
                matchBehavioralDrugs = behavioralFactors.drugs == true ? 1 : 0,
                matchBehavioralDistraction = behavioralFactors.distraction == true ? 1 : 0,
                matchBehavioralAggressive = behavioralFactors.aggressiveDriving == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCommonViolationPredicate(CommonViolations commonViolations)
        {
            // TODO: tag crashes with common violation types in oracle

            // test for valid filter
            if (commonViolations == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
              SELECT NULL FROM {_warehouseSchema}.FACT_VIOLATION
              WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_VIOLATION.HSMV_RPT_NBR
              AND (
                (:matchViolationSpeed = 1 AND UPPER(FACT_VIOLATION.CHARGE) LIKE '%SPEED%')
                OR (
                  :matchViolationRightOfWay = 1
                  AND (
                    UPPER(FACT_VIOLATION.CHARGE) LIKE '%FAIL% TO YIELD%'
                    OR UPPER(FACT_VIOLATION.CHARGE) LIKE '%RIGHT OF WAY%'
                  )
                )
                OR (:matchViolationTrafficControl = 1 AND UPPER(FACT_VIOLATION.CHARGE) LIKE '%TRAFFIC CONTROL%')
                OR (:matchViolationCarelessDriving = 1 AND UPPER(FACT_VIOLATION.CHARGE) LIKE '%CARELESS%')
                OR (
                  :matchViolationDui = 1
                  AND (
                    REGEXP_LIKE(FACT_VIOLATION.FL_STATUTE_NBR, '^316(\.|-|\,| |)193(\.|-|\,| |\(|$)')
                    OR (
                      REGEXP_LIKE(UPPER(FACT_VIOLATION.FL_STATUTE_NBR), '^(|CHAPTER )316$')
                      AND REGEXP_LIKE(UPPER(FACT_VIOLATION.CHARGE), '^D(|\.| )U(|\.| )I')
                    )
                  )
                )
                OR (:matchViolationRedLight = 1 AND UPPER(FACT_VIOLATION.FL_STATUTE_NBR) LIKE '316%075%1%C%')
              )
            )
            OR (
              :matchViolationRedLight = 1
              AND EXISTS (
                SELECT NULL FROM {_warehouseSchema}.FACT_DRIVER FD
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FD.HSMV_RPT_NBR
                AND ( FD.KEY_ACTION1 = 7 OR FD.KEY_ACTION2 = 7 OR FD.KEY_ACTION3 = 7 OR FD.KEY_ACTION4 = 7 )
              )
            )";

            // define oracle parameters
            var parameters = new {
                matchViolationSpeed = commonViolations.speed == true ? 1 : 0,
                matchViolationRedLight = commonViolations.redLight == true ? 1 : 0,
                matchViolationRightOfWay = commonViolations.rightOfWay == true ? 1 : 0,
                matchViolationTrafficControl = commonViolations.trafficControlDevice == true ? 1 : 0,
                matchViolationCarelessDriving = commonViolations.carelessDriving == true ? 1 : 0,
                matchViolationDui = commonViolations.dui == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateVehicleTypePredicate(IList<int> vehicleType)
        {
            // test for valid filter
            if (vehicleType == null || vehicleType.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.V_FACT_ALL_VEH
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = V_FACT_ALL_VEH.HSMV_RPT_NBR
                AND V_FACT_ALL_VEH.KEY_VEH_TYPE IN :vehicleTypes
            )";

            // define oracle parameters
            var parameters = new { vehicleTypes = vehicleType };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCrashTypeSimplePredicate(IList<string> crashTypeSimple)
        {
            // test for valid filter
            if (crashTypeSimple == null || crashTypeSimple.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.V_CRASH_TYPE_SIMPLIFIED
                WHERE FACT_CRASH_EVT.KEY_CRASH_TYPE = V_CRASH_TYPE_SIMPLIFIED.ID
                AND V_CRASH_TYPE_SIMPLIFIED.CRASH_ATTR_TX IN :crashTypeSimple
            )";

            // define oracle parameters
            var parameters = new {
                crashTypeSimple
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCrashTypeDetailedPredicate(IList<int> crashTypeDetailed)
        {
            // test for valid filter
            if (crashTypeDetailed == null || crashTypeDetailed.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.V_CRASH_TYPE
                WHERE FACT_CRASH_EVT.KEY_CRASH_TYPE = V_CRASH_TYPE.ID
                AND V_CRASH_TYPE.ID IN :crashTypeDetailed
            )";

            // define oracle parameters
            var parameters = new { crashTypeDetailed };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateBikePedCrashTypePredicate(BikePedCrashType bikePedCrashType)
        {
            // test for valid filter
            if (bikePedCrashType == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_BIKE_PED_CRASH_TYPE IN :bikePedCrashType
            OR (
                :matchBikePedUntyped = 1
                AND (
                    FACT_CRASH_EVT.PED_CNT > 0
                    OR FACT_CRASH_EVT.BIKE_CNT > 0
                    OR DIM_HARMFUL_EVT.HARMFUL_EVT_TX IN ('Pedestrian', 'Pedalcycle')
                )
                AND FACT_CRASH_EVT.KEY_BIKE_PED_CRASH_TYPE IS NULL
            )";

            // define oracle parameters
            var parameters = new {
                bikePedCrashType.bikePedCrashTypeIds,
                matchBikePedUntyped = bikePedCrashType.includeUntyped == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCmvConfigurationPredicate(IList<int> cmvConfiguration)
        {
            // test for valid filter
            if (cmvConfiguration == null || cmvConfiguration.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"EXISTS (
                SELECT NULL FROM {_warehouseSchema}.FACT_COMM_VEH
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_COMM_VEH.HSMV_RPT_NBR
                AND FACT_COMM_VEH.KEY_CMV_CONFIG IN :cmvConfiguration
            )";

            // define oracle parameters
            var parameters = new { cmvConfiguration };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateEnvironmentalCircumstancePredicate(IList<int> environmentalCircumstance)
        {
            // test for valid filter
            if (environmentalCircumstance == null || environmentalCircumstance.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_ENV1 IN :environmentalCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_ENV2 IN :environmentalCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_ENV3 IN :environmentalCircumstance";

            // define oracle parameters
            var parameters = new { environmentalCircumstance };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateRoadCircumstancePredicate(IList<int> roadCircumstance)
        {
            // test for valid filter
            if (roadCircumstance == null || roadCircumstance.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_RD1 IN :roadCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_RD2 IN :roadCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_RD3 IN :roadCircumstance";

            // define oracle parameters
            var parameters = new { roadCircumstance };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateFirstHarmfulEventPredicate(IList<int> firstHarmfulEvent)
        {
            // test for valid filter
            if (firstHarmfulEvent == null || firstHarmfulEvent.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_1ST_HE IN :firstHarmfulEvent";

            // define oracle parameters
            var parameters = new { firstHarmfulEvent };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateLightConditionPredicate(IList<int> lightCondition)
        {
            // test for valid filter
            if (lightCondition == null || lightCondition.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_LIGHT_COND IN :lightCondition";

            // define oracle parameters
            var parameters = new { lightCondition };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateRoadSystemIdentifierPredicate(IList<int> roadSystemIdentifier)
        {
            // test for valid filter
            if (roadSystemIdentifier == null || roadSystemIdentifier.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_RD_SYS_ID IN :roadSystemIdentifier";

            // define oracle parameters
            var parameters = new { roadSystemIdentifier };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateWeatherConditionPredicate(IList<int> weatherCondition)
        {
            // test for valid filter
            if (weatherCondition == null || weatherCondition.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_WEATHER_COND IN :weatherCondition";

            // define oracle parameters
            var parameters = new { weatherCondition };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateLaneDeparturePredicate(LaneDepartures laneDepartures)
        {
            // TODO: tag crashes with lane departure in oracle
            // TODO: for off-road, we should join V_CRASH_TYPE and filter on V_CRASH_TYPE.CRASH_ATTR_CD = 6
            // TODO: for collision with fixed object, we should join DIM_HARMFUL_EVT and filter on DIM_HARMFUL_EVT.HARMFUL_EVT_CD BETWEEN 19 AND 39 (except 20)

            // test for valid filter
            if (laneDepartures == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"(:matchLaneDepartureOffRoad = 1 AND FACT_CRASH_EVT.KEY_CRASH_TYPE = 45)
                OR (
                    :matchLaneDepartureRollover = 1
                    AND FACT_CRASH_EVT.KEY_CRASH_TYPE = 45
                    AND (
                        EXISTS (
                            SELECT NULL FROM {_warehouseSchema}.V_FACT_ALL_VEH
                            WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = V_FACT_ALL_VEH.HSMV_RPT_NBR
                            AND (
                                V_FACT_ALL_VEH.KEY_MOST_HE = 1
                                OR V_FACT_ALL_VEH.KEY_HE1 = 1
                                OR V_FACT_ALL_VEH.KEY_HE2 = 1
                                OR V_FACT_ALL_VEH.KEY_HE3 = 1
                                OR V_FACT_ALL_VEH.KEY_HE4 = 1
                            )
                        )
                    )
                )
                OR (
                    :matchLaneDepartureCollision = 1
                    AND FACT_CRASH_EVT.KEY_CRASH_TYPE = 45
                    AND (
                        EXISTS (
                            SELECT NULL FROM {_warehouseSchema}.V_FACT_ALL_VEH
                            WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = V_FACT_ALL_VEH.HSMV_RPT_NBR
                            AND (
                                V_FACT_ALL_VEH.KEY_MOST_HE IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR V_FACT_ALL_VEH.KEY_HE1 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR V_FACT_ALL_VEH.KEY_HE2 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR V_FACT_ALL_VEH.KEY_HE3 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR V_FACT_ALL_VEH.KEY_HE4 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                            )
                        )
                    )
                )
                OR (:matchLaneDepartureOncoming = 1 AND FACT_CRASH_EVT.KEY_CRASH_TYPE = 41)
                OR (:matchLaneDepartureSideswipe = 1 AND FACT_CRASH_EVT.KEY_CRASH_TYPE IN (46,55))";

            // define oracle parameters
            var parameters = new {
                matchLaneDepartureOffRoad = laneDepartures.offRoadAll == true ? 1 : 0,
                matchLaneDepartureRollover = laneDepartures.offRoadRollover == true ? 1 : 0,
                matchLaneDepartureCollision = laneDepartures.offRoadCollisionWithFixedObject == true ? 1 : 0,
                matchLaneDepartureOncoming = laneDepartures.crossedIntoOncomingTraffic == true ? 1 : 0,
                matchLaneDepartureSideswipe = laneDepartures.sideswipe == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateOtherCircumstancePredicate(OtherCircumstances otherCircumstances)
        {
            // test for valid filter
            if (otherCircumstances == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = $@"(:matchSchoolBusRelated = 1 AND FACT_CRASH_EVT.IS_SCH_BUS_REL = '1')
                OR (
                  :matchWithinCityLimits = 1
                  AND (
                    FACT_CRASH_EVT.IS_WITHIN_CITY_LIM = '1'
                    OR MOD(GEOCODE_RESULT.KEY_GEOGRAPHY, 100) <> 0
                  )
                )
                OR (:matchWithinInterchange = 1 AND FACT_CRASH_EVT.IS_1ST_HE_WITHIN_INTRCHG = '1')
                OR (:matchWorkZoneRelated = 1 AND FACT_CRASH_EVT.IS_WORK_ZN_REL = '1')
                OR (:matchWorkersInWorkZone = 1 AND FACT_CRASH_EVT.IS_WORKERS_IN_WORK_ZN = '1')
                OR (:matchLawEnforcementInWorkZone = 1 AND FACT_CRASH_EVT.IS_LE_IN_WORK_ZN = '1')
                OR (
                  :matchHitAndRun = 1
                  AND EXISTS (
                    SELECT NULL FROM {_warehouseSchema}.V_FACT_ALL_VEH
                    WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = V_FACT_ALL_VEH.HSMV_RPT_NBR
                    AND V_FACT_ALL_VEH.IS_HIT_AND_RUN = '1'
                  )
                )";

            // define oracle parameters
            var parameters = new {
                matchSchoolBusRelated = otherCircumstances.schoolBusRelated == true ? 1 : 0,
                matchWithinCityLimits = otherCircumstances.withinCityLimits == true ? 1 : 0,
                matchWithinInterchange = otherCircumstances.withinInterchange == true ? 1 : 0,
                matchWorkZoneRelated = otherCircumstances.workZoneRelated == true ? 1 : 0,
                matchWorkersInWorkZone = otherCircumstances.workersInWorkZone == true ? 1 : 0,
                matchLawEnforcementInWorkZone = otherCircumstances.lawEnforcementInWorkZone == true ? 1 : 0,
                matchHitAndRun = otherCircumstances.hitAndRun == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        /*
        TEMPLATE:

        private (string whereClause, object parameters) GenerateXPredicate(IList<int> filter)
        {
            // test for valid filter
            if (filter == null || filter.Count == 0)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }
        */
    }
}
