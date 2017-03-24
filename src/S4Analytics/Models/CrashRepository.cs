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
            return new List<CrashResult>();
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
            // TODO: don't hard code spatial schema name

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
            var ped = query.nonAutoModesOfTravel.pedestrian == true;
            var bike = query.nonAutoModesOfTravel.bicyclist == true;
            var moped = query.nonAutoModesOfTravel.moped == true;
            var motorcycle = query.nonAutoModesOfTravel.motorcycle == true;

            // define where clause
            var whereClause = @"(:matchNonAutoModePed = 1 AND FACT_CRASH_EVT.PED_CNT > 0)
                OR (:matchNonAutoModeBike = 1 AND FACT_CRASH_EVT.BIKE_CNT > 0)
                OR (:matchNonAutoModeMoped = 1 AND FACT_CRASH_EVT.MOPED_CNT > 0)
                OR (:matchNonAutoModeMotorcycle = 1 AND FACT_CRASH_EVT.MOTORCYCLE_CNT > 0)";

            // define oracle parameters
            var parameters = new {
                matchNonAutoModePed = ped ? 1 : 0,
                matchNonAutoModeBike = bike ? 1 : 0,
                matchNonAutoModeMoped = moped ? 1 : 0,
                matchNonAutoModeMotorcycle = motorcycle ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateSourceOfTransportPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var ems = query.sourcesOfTransport.ems == true;
            var lawEnforcement = query.sourcesOfTransport.lawEnforcement == true;
            var other = query.sourcesOfTransport.other == true;

            // define where clause
            var whereClause = @"(:matchEmsTransport = 1 AND FACT_CRASH_EVT.TRANS_BY_EMS_CNT > 0)
                OR (:matchLawEnforcementTransport = 1 AND FACT_CRASH_EVT.TRANS_BY_LE_CNT > 0)
                OR (:matchOtherTransport = 1 AND FACT_CRASH_EVT.TRANS_BY_OTH_CNT > 0)";

            // define oracle parameters
            var parameters = new {
                matchEmsTransport = ems ? 1 : 0,
                matchLawEnforcementTransport = lawEnforcement ? 1 : 0,
                matchOtherTransport = other ? 1 : 0
            };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateBehavioralFactorPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var behavioralFactors = query.behavioralFactors;

            // define where clause
            var whereClause = "";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }

        /*
        TEMPLATE:

        private (string whereClause, object parameters) GenerateXPredicate(CrashQuery query)
        {
            // isolate relevant filter
            var x = query.x;

            // define where clause
            var whereClause = "";

            // define oracle parameters
            var parameters = new { };

            return (whereClause, parameters);
        }
        */
    }
}
