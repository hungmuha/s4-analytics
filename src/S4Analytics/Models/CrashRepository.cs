using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class CrashRepository : ICrashRepository
    {
        private string _connStr;
        private readonly IList<string> pseudoEmptyStringList = new List<string>() { "" };
        private readonly IList<int> pseudoEmptyIntList = new List<int>() { -1 };

        public CrashRepository(IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.WarehouseConnStr;
        }

        public int CreateQuery(CrashQuery query) {
            using (var conn = new OracleConnection(_connStr))
            {
                // get query id
                var cmdText = "SELECT crash_query_seq.nextval FROM dual";
                var queryId = conn.QuerySingle<int>(cmdText);

                // populate crash query table with matching crash ids
                DynamicParameters parameters;
                (cmdText, parameters) = ConstructCrashQuery(queryId, query);
                conn.Execute(cmdText, parameters);

                return queryId;
            }
        }

        public bool QueryExists(int queryId)
        {
            using (var conn = new OracleConnection(_connStr))
            {
                var cmdText = "SELECT 1 FROM crash_query WHERE id = :queryId";
                var exists = conn.QuerySingleOrDefault<int>(cmdText, new { queryId }) == 1;
                return exists;
            }
        }

        public IEnumerable<CrashResult> GetCrashes(int queryId) {
            var queryText = @"SELECT
              /* geocode_result.map_point_x,
              geocode_result.map_point_y,
              geocode_result.center_line_x,
              geocode_result.center_line_y,
              geocode_result.crash_seg_id,
              geocode_result.nearest_intrsect_id,
              geocode_result.nearest_intrsect_offset_ft,
              geocode_result.nearest_intrsect_offset_dir,
              geocode_result.ref_intrsect_id,
              geocode_result.ref_intrsect_offset_ft,
              geocode_result.ref_intrsect_offset_dir, */
              fact_crash_evt.hsmv_rpt_nbr AS Id,
              fact_crash_evt.hsmv_rpt_nbr AS HsmvReportNumber /*,
              fact_crash_evt.key_crash_dt crash_date,
              fact_crash_evt.key_crash_sev,
              fact_crash_evt.key_crash_type,
              v_crash_sev.crash_attr_tx crash_severity,
              v_crash_type.crash_attr_tx crash_type,
              v_crash_light_cond.crash_attr_tx light_cond,
              v_crash_weather_cond.crash_attr_tx weather_cond,
              fact_crash_evt.crash_tm crash_time,
              dim_geography.cnty_nm county,
              dim_geography.city_nm city,
              geocode_result.st_nm street_name,
              geocode_result.intrsect_st_nm intersecting_street,
              fact_crash_evt.is_alc_rel is_alcohol_related,
              fact_crash_evt.is_distracted,
              fact_crash_evt.is_drug_rel is_drug_related,
              fact_crash_evt.lat,
              fact_crash_evt.lng,
              fact_crash_evt.offset_dir,
              fact_crash_evt.offset_ft,
              fact_crash_evt.veh_cnt vehicle_count,
              fact_crash_evt.nm_cnt nonmotorist_count,
              fact_crash_evt.fatality_cnt fatality_count,
              fact_crash_evt.inj_cnt injury_count,
              fact_crash_evt.tot_dmg_amt,
              dim_agncy.agncy_short_nm,
              geocode_result.cnty_cd cnty_cd,
              geocode_result.city_cd city_cd,
              fact_crash_evt.agncy_rpt_nbr agency_report_number,
              decode(fact_crash_evt.form_type_cd, 'L', 'Long', 'S', 'Short', '') form_type,
              v_crash_type_simplified.crash_attr_tx crash_type_simple,
              fact_crash_evt.crash_type_dir_tx,
              v_crash_road_surf_cond.crash_attr_tx,
              dim_harmful_evt.harmful_evt_tx,
              fact_crash_evt.img_ext_tx,
              geocode_result.sym_angle,
              dim_agncy.ID,
              CASE
                WHEN v_bike_ped_crash_type.bike_or_ped IS NOT NULL THEN v_bike_ped_crash_type.bike_or_ped
                WHEN (fact_crash_evt.ped_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedestrian') AND (fact_crash_evt.bike_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedalcycle') THEN '?'
                WHEN fact_crash_evt.ped_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedestrian' THEN 'P'
                WHEN fact_crash_evt.bike_cnt > 0 OR dim_harmful_evt.harmful_evt_tx = 'Pedalcycle' THEN 'B'
              END AS bike_or_ped,
              v_bike_ped_crash_type.crash_type_nm AS bike_ped_crash_type_nm,
              fact_crash_evt.bike_cnt,
              fact_crash_evt.ped_cnt */
            FROM fact_crash_evt
            INNER JOIN crash_query
              ON crash_query.hsmv_rpt_nbr = fact_crash_evt.hsmv_rpt_nbr
            INNER JOIN navteq_2015q1.geocode_result
              ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
            LEFT JOIN dim_agncy
              ON fact_crash_evt.key_rptg_agncy = dim_agncy.ID
            LEFT JOIN dim_geography
              ON fact_crash_evt.key_geography = dim_geography.ID
            LEFT JOIN v_crash_sev
              ON fact_crash_evt.key_crash_sev = v_crash_sev.ID
            LEFT JOIN v_crash_weather_cond
              ON fact_crash_evt.key_weather_cond = v_crash_weather_cond.ID
            LEFT JOIN v_crash_light_cond
              ON fact_crash_evt.key_light_cond = v_crash_light_cond.ID
            LEFT JOIN v_crash_type_simplified
              ON fact_crash_evt.key_crash_type = v_crash_type_simplified.ID
            LEFT JOIN v_crash_type
              ON fact_crash_evt.key_crash_type = v_crash_type.ID
            LEFT JOIN v_crash_road_surf_cond
              ON fact_crash_evt.key_rd_surf_cond = v_crash_road_surf_cond.ID
            LEFT JOIN dim_harmful_evt
              ON fact_crash_evt.key_1st_he = dim_harmful_evt.ID
            LEFT JOIN v_bike_ped_crash_type
              ON fact_crash_evt.key_bike_ped_crash_type = v_bike_ped_crash_type.crash_type_id
            WHERE crash_query.id = :queryId";

            using (var conn = new OracleConnection(_connStr))
            {
                var crashResults = conn.Query<CrashResult>(queryText, new { queryId });
                return crashResults;
            }
        }

        private (string, DynamicParameters) ConstructCrashQuery(int queryId, CrashQuery query)
        {
            var queryText = @"INSERT INTO crash_query (id, crash_id)
                SELECT
                  :queryId,
                  fact_crash_evt.hsmv_rpt_nbr
                FROM s4_warehouse.fact_crash_evt
                INNER JOIN navteq_2015q1.geocode_result
                  ON fact_crash_evt.hsmv_rpt_nbr = geocode_result.hsmv_rpt_nbr
                LEFT JOIN s4_warehouse.dim_harmful_evt
                  ON fact_crash_evt.key_1st_he = dim_harmful_evt.ID";
            var initialParameters = new { queryId };

            // initialize where clause and query parameter collections
            var whereClauses = new List<string>();
            var queryParameters = new DynamicParameters(initialParameters);

            // populate list with generator methods for all valid filters
            var predicateMethods = GetPredicateMethods(query);

            // generate where clause and query parameters for each valid filter
            predicateMethods.ForEach(generatePredicate => {
                (var whereClause, var parameters) = generatePredicate.Invoke(query);
                if (whereClause != null)
                {
                    whereClauses.Add(whereClause);
                    queryParameters.AddDynamicParams(parameters);
                }
            });

            // join where clauses; append to insert statement
            queryText += " WHERE (" + string.Join(") AND (", whereClauses) + ")";

            return (queryText, queryParameters);
        }

        private List<Func<CrashQuery, (string, object)>> GetPredicateMethods(CrashQuery query)
        {
            bool hasDateRange = query.dateRange != null;
            bool hasDayOfWeek = query.dayOfWeek != null && query.dayOfWeek.Count > 0;
            bool hasTimeRange = query.timeRange != null;
            bool hasDotDistrict = query.dotDistrict != null && query.dotDistrict.Count > 0;
            bool hasMpoTpo = query.mpoTpo != null && query.mpoTpo.Count > 0;

            var predicateMethods = new List<Func<CrashQuery, (string, object)>>();

            if (hasDateRange) { predicateMethods.Add(GenerateDateRangePredicate); }
            if (hasDayOfWeek) { predicateMethods.Add(GenerateDayOfWeekPredicate); }
            if (hasTimeRange) { predicateMethods.Add(GenerateTimeRangePredicate); }

            return predicateMethods;
        }

        private (string whereClause, object parameters) GenerateDateRangePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var dateRange = query.dateRange;

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

        private (string whereClause, object parameters) GenerateDayOfWeekPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var dayOfWeek = query.dayOfWeek;

            // define where clause
            var whereClause = "TO_CHAR(FACT_CRASH_EVT.KEY_CRASH_DT, 'D') IN :dayOfWeek";

            // define oracle parameters
            var parameters = new { dayOfWeek };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateTimeRangePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var startTime = query.timeRange.startTime;
            var endTime = query.timeRange.endTime;

            // define where clause
            var whereClause = @"(
                :startTime <= :endTime -- same day
                AND TO_CHAR(CRASH_TM, 'HH24MI') BETWEEN :startTime AND :endTime
            ) OR (
                :startTime > :endTime -- crosses midnight boundary
                AND TO_CHAR(CRASH_TM, 'HH24MI') >= :startTime OR TO_CHAR(CRASH_TM, 'HH24MI') <= :endTime
            )";

            // define oracle parameters
            var parameters = new {
                startTime = startTime.Hour*100 + startTime.Minute,
                endTime = endTime.Hour*100 + endTime.Minute
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDotDistrictPredicate(CrashQuery query)
        {
            // TODO: tag crashes with dot district in oracle

            // isolate relevant filter
            var dotDistrict = query.dotDistrict;

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

        private (string whereClause, object parameters) GenerateMpoTpoPredicate(CrashQuery query)
        {
            // TODO: tag crashes with mpo/tpo in oracle

            // isolate relevant filter
            var mpoTpo = query.mpoTpo;

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
            var whereClause = @"FLOOR(FACT_CRASH_EVT.KEY_GEOGRAPHY / 100) IN :mpoCountyCodes
                OR FLOOR(GEOCODE_RESULT.KEY_GEOGRAPHY / 100) IN :mpoCountyCodes
                OR EXISTS (
                    SELECT NULL FROM navteq_2015q1.ST_EXT
                    WHERE LINK_ID = GEOCODE_RESULT.CRASH_SEG_ID
                    AND MPO_BND_ID IN :partialCountyMpoIds
                )";

            // define oracle parameters
            var parameters = new {
                mpoCountyCodes = mpoCountyCodes.Any() ? mpoCountyCodes : pseudoEmptyIntList,
                partialCountyMpoIds = partialCountyMpoIds.Any() ? partialCountyMpoIds : pseudoEmptyIntList
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCountyPredicate(CrashQuery query)
        {
            // TODO: join to dim_geography rather than divide by 100 in where clause

            // isolate relevant filter
            var county = query.county;

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
                fullCountyCodes = fullCountyCodes.Any() ? fullCountyCodes : pseudoEmptyIntList,
                unincorporatedCountyCodes = unincorporatedCountyCodes.Any() ? unincorporatedCountyCodes : pseudoEmptyIntList
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCityPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var city = query.city;

            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_GEOGRAPHY IN :cityCodes OR GEOCODE_RESULT.KEY_GEOGRAPHY IN :cityCodes";

            // define oracle parameters
            var parameters = new {
                cityCodes = city
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCustomAreaPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var customArea = query.customArea;

            var customAreaMinX = customArea.Min(coords => coords.x);
            var customAreaMinY = customArea.Min(coords => coords.y);
            var customAreaMaxX = customArea.Max(coords => coords.x);
            var customAreaMaxY = customArea.Max(coords => coords.y);
            var coordsAsText = customArea.Select(coords => string.Format("{0} {1}", coords.x, coords.y));
            var customAreaPolygon = "POLYGON ((" + string.Join(", ", coordsAsText) + "))";
            var customAreaSrid = 1234; // TODO: get SRID from somewhere

            // define where clause
            var whereClause = @"GEOCODE_RESULT.MAP_POINT_X BETWEEN :customAreaMinX AND :customAreaMaxX
                AND GEOCODE_RESULT.MAP_POINT_Y BETWEEN :customAreaMinY AND :customAreaMaxY
                AND SDE.ST_WITHIN(GEOCODE_RESULT.SHAPE, SDE.ST_GEOMETRY(:customAreaPolygon, :customAreaSrid)) = 1";

            // define oracle parameters
            var parameters = new {
                customAreaMinX, customAreaMinY, customAreaMaxX, customAreaMaxY,
                customAreaPolygon, customAreaSrid
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCustomExtentPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var customExtent = query.customExtent;

            var customExtentMinX = Math.Min(customExtent.point1.x, customExtent.point2.x);
            var customExtentMinY = Math.Min(customExtent.point1.y, customExtent.point2.y);
            var customExtentMaxX = Math.Max(customExtent.point1.x, customExtent.point2.x);
            var customExtentMaxY = Math.Max(customExtent.point1.y, customExtent.point2.y);

            // define where clause
            var whereClause = @"GEOCODE_RESULT.MAP_POINT_X BETWEEN :customExtentMinX AND :customExtentMaxX
                AND GEOCODE_RESULT.MAP_POINT_Y BETWEEN :customExtentMinY AND :customExtentMaxY";

            // define oracle parameters
            var parameters = new {
                customExtentMinX, customExtentMinY, customExtentMaxX, customExtentMaxY
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateIntersectionPredicate(CrashQuery query)
        {
            // TODO: don't hard code schema names

            // isolate relevant filter
            var intersection = query.intersection;

            var unknownOffsetDir = intersection.offsetDirection.Any(dir => dir == "Unknown");
            var intersectionOffsetDirs = intersection.offsetDirection.Where(dir => dir != "Unknown").ToList();

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM navteq_2015q1.GEOCODE_RESULT
                WHERE GEOCODE_RESULT.HSMV_RPT_NBR = FACT_CRASH_EVT.HSMV_RPT_NBR
                AND GEOCODE_RESULT.NEAREST_INTRSECT_ID = :intersectionId
                AND GEOCODE_RESULT.NEAREST_INTRSECT_OFFSET_FT <= :intersectionOffsetFeet
            ) AND (
                FACT_CRASH_EVT.OFFSET_DIR IN :intersectionOffsetDirs
                OR (:matchUnknownOffsetDir = 1 AND FACT_CRASH_EVT.OFFSET_DIR IS NULL)
            )";

            // define oracle parameters
            var parameters = new {
                intersection.intersectionId,
                intersectionOffsetFeet = intersection.offsetInFeet,
                intersectionOffsetDirs = intersectionOffsetDirs.Any() ? intersectionOffsetDirs : pseudoEmptyStringList,
                matchUnknownOffsetDir = unknownOffsetDir ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateStreetPredicate(CrashQuery query)
        {
            // TODO: refactor query to use WHERE EXISTS instead of IN

            // isolate relevant filter
            var street = query.street;

            // define where clause
            var whereClause = "GEOCODE_RESULT.CRASH_SEG_ID IN :streetLinkIds";
            if (street.includeCrossStreets)
            {
                whereClause += @" OR (
                    GEOCODE_RESULT.NEAREST_INTRSECT_OFFSET_FT <= 100
                    AND GEOCODE_RESULT.NEAREST_INTRSECT_ID IN (
                    SELECT DISTINCT INTRSECT_NODE.INTERSECTION_ID
                    FROM navteq_2015q1.INTRSECT_NODE
                    WHERE INTRSECT_NODE.NODE_ID IN (
                        SELECT ST.REF_IN_ID
                        FROM navteq_2015q1.ST
                        WHERE ST.LINK_ID IN :streetLinkIds
                        UNION
                        SELECT ST.NREF_IN_ID
                        FROM navteq_2015q1.ST
                        WHERE ST.LINK_ID IN :streetLinkIds
                    )
                  )
                )";
            }

            // define oracle parameters
            var parameters = new {
                streetLinkIds = street.linkIds
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCustomNetworkPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var customNetwork = query.customNetwork;

            // define where clause
            var whereClause = "GEOCODE_RESULT.CRASH_SEG_ID IN :customNetworkLinkIds";

            // define oracle parameters
            var parameters = new {
                customNetworkLinkIds = customNetwork
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GeneratePublicRoadPredicate(CrashQuery query)
        {
            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_RD_SYS_ID IN (140,141,142,143,144,145)";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateFormTypePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var formType = query.formType;

            // define where clause
            var whereClause = "FACT_CRASH_EVT.FORM_TYPE_CD IN :formType";

            // define oracle parameters
            var parameters = new { formType };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCodeablePredicate(CrashQuery query)
        {
            // define where clause
            var whereClause = "FACT_CRASH_EVT.CODEABLE = 'T'";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateReportingAgencyPredicate(CrashQuery query)
        {
            // TODO: move fhp troop map to oracle

            // isolate relevant filter
            var reportingAgency = query.reportingAgency;

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
                agencyIds = agencyIds.Any() ? agencyIds : pseudoEmptyIntList,
                fhpTroops = fhpTroops.Any() ? fhpTroops : pseudoEmptyStringList
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDriverGenderPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var driverGender = query.driverGender;

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM FACT_DRIVER
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_DRIVER.HSMV_RPT_NBR
                AND FACT_DRIVER.KEY_GENDER IN :driverGender
            )";

            // define oracle parameters
            var parameters = new { driverGender };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDriverAgePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var driverAgeRange = query.driverAgeRange;

            var unknownAge = driverAgeRange.Where(ageRange => ageRange == "Unknown").Any();
            var ageRanges = driverAgeRange.Where(ageRange => ageRange != "Unknown").ToList();

            // define where clause
            var whereClause = @"(:matchUnknownDriverAge = 1
                AND NOT EXISTS (
                    SELECT NULL FROM FACT_DRIVER
                    WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_DRIVER.HSMV_RPT_NBR
                )
            ) OR EXISTS (
                SELECT NULL FROM s4_warehouse.FACT_DRIVER
                LEFT OUTER JOIN s4_warehouse.V_DRIVER_AGE_RNG
                    ON FACT_DRIVER.KEY_AGE_RNG = V_DRIVER_AGE_RNG.ID
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_DRIVER.HSMV_RPT_NBR
                AND (
                    V_DRIVER_AGE_RNG.NM_ATTR_TX IN :driverAgeRanges
                    OR (:matchUnknownDriverAge = 1 AND V_DRIVER_AGE_RNG.NM_ATTR_TX IS NULL)
                )
            )";

            // define oracle parameters
            var parameters = new {
                driverAgeRanges = ageRanges.Any() ? ageRanges : pseudoEmptyStringList,
                matchUnknownDriverAge = unknownAge ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GeneratePedestrianAgePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var pedestrianAgeRange = query.pedestrianAgeRange;

            var unknownAge = pedestrianAgeRange.Where(ageRange => ageRange == "Unknown").Any();
            var ageRanges = pedestrianAgeRange.Where(ageRange => ageRange != "Unknown").ToList();

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM s4_warehouse.FACT_NON_MOTORIST
                LEFT OUTER JOIN s4_warehouse.V_NM_AGE_RNG
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
                pedestrianAgeRanges = ageRanges.Any() ? ageRanges : pseudoEmptyStringList,
                matchUnknownPedestrianAge = unknownAge ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCyclistAgePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var cyclistAgeRange = query.cyclistAgeRange;

            var unknownAge = cyclistAgeRange.Where(ageRange => ageRange == "Unknown").Any();
            var ageRanges = cyclistAgeRange.Where(ageRange => ageRange != "Unknown").ToList();

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM s4_warehouse.FACT_NON_MOTORIST
                LEFT OUTER JOIN s4_warehouse.V_NM_AGE_RNG
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
                cyclistAgeRanges = ageRanges.Any() ? ageRanges : pseudoEmptyStringList,
                matchUnknownCyclistAge = unknownAge ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateNonAutoModeOfTravelPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var nonAutoModesOfTravel = query.nonAutoModesOfTravel;

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

        private (string whereClause, object parameters) GenerateSourceOfTransportPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var sourcesOfTransport = query.sourcesOfTransport;

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

        private (string whereClause, object parameters) GenerateBehavioralFactorPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var behavioralFactors = query.behavioralFactors;

            // define where clause
            var whereClause = @"(:matchBehavioralAlcohol = 1 AND FACT_CRASH_EVT.IS_ALC_REL = '1')
                OR (:matchBehavioralDrugs = 1 AND FACT_CRASH_EVT.IS_DRUG_REL = '1')
                OR (:matchBehavioralDistraction = 1 AND FACT_CRASH_EVT.IS_DISTRACTED = '1')
                OR (:matchBehavioralAggressiveDriving = 1 AND FACT_CRASH_EVT.IS_AGGRESSIVE = '1')";

            // define oracle parameters
            var parameters = new {
                matchBehavioralAlcohol = behavioralFactors.alcohol == true ? 1 : 0,
                matchBehavioralDrugs = behavioralFactors.drugs == true ? 1 : 0,
                matchBehavioralDistraction = behavioralFactors.distraction == true ? 1 : 0,
                matchBehavioralAggressiveDriving = behavioralFactors.aggressiveDriving == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCommonViolationPredicate(CrashQuery query)
        {
            // TODO: tag crashes with common violation types in oracle

            // isolate relevant filter
            var commonViolations = query.commonViolations;

            // define where clause
            var whereClause = @"EXISTS (
              SELECT NULL FROM s4_warehouse.FACT_VIOLATION
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
                OR (:matchViolationTrafficControlDevice = 1 AND UPPER(FACT_VIOLATION.CHARGE) LIKE '%TRAFFIC CONTROL%')
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
                OR (:matchViolationRedLight = 1 AND OR UPPER(FACT_VIOLATION.FL_STATUTE_NBR) LIKE '316%075%1%C%')
              )
            )
            OR (
              :matchViolationRedLight = 1
              AND EXISTS (
                SELECT NULL FROM s4_warehouse.FACT_DRIVER FD
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FD.HSMV_RPT_NBR
                AND ( FD.KEY_ACTION1 = 7 OR FD.KEY_ACTION2 = 7 OR FD.KEY_ACTION3 = 7 OR FD.KEY_ACTION4 = 7 )
              )
            )";

            // define oracle parameters
            var parameters = new {
                matchViolationSpeed = commonViolations.speed == true ? 1 : 0,
                matchViolationRedLight = commonViolations.redLight == true ? 1 : 0,
                matchViolationRightOfWay = commonViolations.rightOfWay == true ? 1 : 0,
                matchViolationTrafficControlDevice = commonViolations.trafficControlDevice == true ? 1 : 0,
                matchViolationCarelessDriving = commonViolations.carelessDriving == true ? 1 : 0,
                matchViolationDui = commonViolations.dui == true ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateVehicleTypePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var vehicleType = query.vehicleType;

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM s4_warehouse.V_FACT_ALL_VEH
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = V_FACT_ALL_VEH.HSMV_RPT_NBR
                AND V_FACT_ALL_VEH.KEY_VEH_TYPE IN :vehicleTypes
            )";

            // define oracle parameters
            var parameters = new { vehicleTypes = vehicleType };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCrashTypeSimplePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var crashTypeSimple = query.crashTypeSimple;

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM s4_warehouse.V_CRASH_TYPE_SIMPLIFIED
                WHERE FACT_CRASH_EVT.KEY_CRASH_TYPE = V_CRASH_TYPE_SIMPLIFIED.ID
                AND V_CRASH_TYPE_SIMPLIFIED.CRASH_ATTR_TX IN :crashTypeSimple
            )";

            // define oracle parameters
            var parameters = new {
                crashTypeSimple
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCrashTypeDetailedPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var crashTypeDetailed = query.crashTypeDetailed;

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM s4_warehouse.V_CRASH_TYPE
                WHERE FACT_CRASH_EVT.KEY_CRASH_TYPE = V_CRASH_TYPE.ID
                AND V_CRASH_TYPE.ID IN :crashTypeDetailed
            )";

            // define oracle parameters
            var parameters = new { crashTypeDetailed };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateBikePedCrashTypePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var bikePedCrashType = query.bikePedCrashType;

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_BIKE_PED_CRASH_TYPE IN :bikePedCrashType
            OR (
                -- not typed yet
                (
                    FACT_CRASH_EVT.PED_CNT > 0
                    OR FACT_CRASH_EVT.BIKE_CNT > 0
                    OR DIM_HARMFUL_EVT.HARMFUL_EVT_TX IN ('Pedestrian', 'Pedalcycle')
                )
                AND FACT_CRASH_EVT.KEY_BIKE_PED_CRASH_TYPE IS NULL
            )";

            // define oracle parameters
            var parameters = new { bikePedCrashType };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateCmvConfigurationPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var cmvConfiguration = query.cmvConfiguration;

            // define where clause
            var whereClause = @"EXISTS (
                SELECT NULL FROM s4_warehouse.FACT_COMM_VEH
                WHERE FACT_CRASH_EVT.HSMV_RPT_NBR = FACT_COMM_VEH.HSMV_RPT_NBR
                AND FACT_COMM_VEH.KEY_CMV_CONFIG IN :cmvConfiguration
            )";

            // define oracle parameters
            var parameters = new { cmvConfiguration };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateEnvironmentalCircumstancePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var environmentalCircumstance = query.environmentalCircumstance;

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_ENV1 IN :environmentalCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_ENV2 IN :environmentalCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_ENV3 IN :environmentalCircumstance";

            // define oracle parameters
            var parameters = new { environmentalCircumstance };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateRoadCircumstancePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var roadCircumstance = query.roadCircumstance;

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_RD1 IN :roadCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_RD2 IN :roadCircumstance
                OR FACT_CRASH_EVT.KEY_CONTRIB_CIRCUM_RD3 IN :roadCircumstance";

            // define oracle parameters
            var parameters = new { roadCircumstance };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateFirstHarmfulEventPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var firstHarmfulEvent = query.firstHarmfulEvent;

            // define where clause
            var whereClause = "FACT_CRASH_EVT.KEY_1ST_HE IN :firstHarmfulEvent";

            // define oracle parameters
            var parameters = new { firstHarmfulEvent };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateLightConditionPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var lightCondition = query.lightCondition;

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_LIGHT_COND IN :lightCondition";

            // define oracle parameters
            var parameters = new { lightCondition };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateRoadSystemIdentifierPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var roadSystemIdentifier = query.roadSystemIdentifier;

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_RD_SYS_ID IN :roadSystemIdentifier";

            // define oracle parameters
            var parameters = new { roadSystemIdentifier };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateWeatherConditionPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var weatherCondition = query.weatherCondition;

            // define where clause
            var whereClause = @"FACT_CRASH_EVT.KEY_WEATHER_COND IN :weatherCondition";

            // define oracle parameters
            var parameters = new { weatherCondition };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateLaneDeparturePredicate(CrashQuery query)
        {
            // TODO: tag crashes with lane departure in oracle
            // TODO: for off-road, we should join V_CRASH_TYPE and filter on V_CRASH_TYPE.CRASH_ATTR_CD = 6
            // TODO: for collision with fixed object, we should join DIM_HARMFUL_EVT and filter on DIM_HARMFUL_EVT.HARMFUL_EVT_CD BETWEEN 19 AND 39 (except 20)

            // isolate relevant filter
            var laneDepartures = query.laneDepartures;

            // define where clause
            var whereClause = @"(:matchLaneDepartureOffRoad = 1 AND FACT_CRASH_EVT.KEY_CRASH_TYPE = 45)
                OR (
                    :matchLaneDepartureRollover = 1
                    AND FACT_CRASH_EVT.KEY_CRASH_TYPE = 45
                    AND (
                        EXISTS (
                            SELECT NULL FROM s4_warehouse.V_FACT_ALL_VEH
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
                            SELECT NULL FROM s4_warehouse.V_FACT_ALL_VEH
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

        private (string whereClause, object parameters) GenerateOtherCircumstancePredicate(CrashQuery query)
        {
            // isolate relevant filter
            var otherCircumstances = query.otherCircumstances;

            // define where clause
            var whereClause = @"(:matchSchoolBusRelated = 1 AND FACT_CRASH_EVT.IS_SCH_BUS_REL = '1')
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
                    SELECT NULL FROM s4_warehouse.V_FACT_ALL_VEH
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

        private (string whereClause, object parameters) GenerateXPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var x = query.x;

            // define where clause
            var whereClause = @"";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }
        */
    }
}
