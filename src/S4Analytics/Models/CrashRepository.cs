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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IList<string> _PSEUDO_EMPTY_STRING_LIST = new List<string>() { "" };
        private readonly IList<int> _PSEUDO_EMPTY_INT_LIST = new List<int>() { -1 };

        public CrashRepository(
            IOptions<ServerOptions> serverOptions,
            IHttpContextAccessor httpContextAccessor)
        {
            _connStr = serverOptions.Value.FlatConnStr;
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
                id,
                key_crash_dt AS crashDate,
                crash_tm AS crashTime,
                hsmv_rpt_nbr_trunc AS hsmvReportNumber,
                agncy_rpt_nbr_trunc AS agencyReportNumber,
                ce1.geocode_pt_3857.sdo_point.x AS mapPointX,
                ce1.geocode_pt_3857.sdo_point.y AS mapPointY,
                sym_angle AS symbolAngle,
                crash_seg_id AS crashSegId,
                nearest_intrsect_id AS nearestIntrsectId,
                nearest_intrsect_offset_ft AS nearestIntrsectOffsetFt,
                ref_intrsect_id AS refIntrsectId,
                ref_intrsect_offset_ft AS refIntrsectOffsetFt,
                nearest_intrsect_offset_dir AS nearIntrsectOffsetDir,
                ref_intrsect_offset_dir AS refIntrsectOffsetDir,
                img_file_nm AS imgFileNm,
                form_type_tx AS formType,
                key_crash_sev AS keyCrashSev,
                key_crash_sev_dtl AS keyCrashSevDtl,
                key_crash_type AS keyCrashType,
                crash_sev AS crashSeverity,
                crash_sev_dtl AS crashSeverityDetailed,
                crash_type_simplified AS crashType,
                crash_type AS crashTypeDetail,
                light_cond AS lightCond,
                weather_cond AS weatherCond,
                cnty_nm AS county,
                city_nm AS city,
                gc_st_nm AS streetName,
                gc_intrsect_st_nm AS intersectingStreet,
                is_alc_rel AS isAlcoholRelated,
                is_distracted AS isDistracted,
                is_drug_rel AS isDrugRelated,
                ce1.gps_pt_4326.sdo_point.x AS lng,
                ce1.gps_pt_4326.sdo_point.y AS lat,
                offset_dir AS offsetDir,
                offset_ft AS offsetFt,
                veh_cnt AS vehicleCount,
                nm_cnt AS nonmotoristCount,
                fatality_cnt AS fatalityCount,
                inj_cnt AS injuryCount,
                tot_dmg_amt AS totDmgAmt,
                rptg_agncy_short_nm AS agncyNm,
                key_rptg_agncy AS agncyId,
                gc_cnty_cd AS cntyCd,
                gc_city_cd AS cityCd,
                crash_type_dir_tx AS crashTypeDir,
                rd_surf_cond AS crashRoadSurfCond,
                first_he AS firstHarmfulEvent,
                bike_or_ped AS bikeOrPed,
                bike_ped_crash_type_nm AS bikePedCrashTypeName,
                bike_cnt AS bikeCount,
                ped_cnt AS pedCount,
                inj_none_cnt as injuryNoneCount,
                inj_possible_cnt AS injuryPossibleCount,
                inj_non_incapacitating_cnt AS injuryNonIncapacitatingCount,
                inj_incapacitating_cnt AS injuryIncapacitatingCount,
                inj_fatal_30_cnt as injuryFatal30Count,
                inj_fatal_non_traffic_cnt AS injuryFatalNonTrafficCount,
                ROWNUM AS rnum
                FROM crash_evt ce1
                INNER JOIN ({preparedQuery.queryText}) pq
                  ON pq.hsmv_rpt_nbr = ce1.hsmv_rpt_nbr
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
                    key_crash_sev_dtl AS sort_order,
                    crash_sev_dtl AS attribute
                  FROM crash_evt ce1
                  INNER JOIN ({preparedQuery.queryText}) pq
                    ON pq.hsmv_rpt_nbr = ce1.hsmv_rpt_nbr
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
                FROM crash_evt ce1
                INNER JOIN ({preparedQuery.queryText}) pq
                  ON pq.hsmv_rpt_nbr = ce1.hsmv_rpt_nbr
                WHERE sdo_inside(ce1.geocode_pt_3857, MDSYS.SDO_GEOMETRY(
                    2003,  -- 2-dimensional polygon
                    3857,  -- SRID
                    NULL,
                    MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,3), -- one rectangle (1003 = exterior)
                    MDSYS.SDO_ORDINATE_ARRAY(
                        :mapExtentMinX, :mapExtentMinY, -- bottom left corner
                        :mapExtentMaxX, :mapExtentMaxY -- top right corner
                    )
                  )) = 'TRUE'";

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
                  FROM crash_evt
                  SAMPLE({samplePercentage})
                )
                SELECT
                  NULL AS eventId, -- do not retrieve ids for sample
                  ce1.geocode_pt_3857.sdo_point.x AS x,
                  ce1.geocode_pt_3857.sdo_point.y AS y
                FROM crash_evt ce1
                INNER JOIN sample_evts
                  ON sample_evts.hsmv_rpt_nbr = ce1.hsmv_rpt_nbr
                INNER JOIN ({preparedQuery.queryText}) pq
                    ON pq.hsmv_rpt_nbr = ce1.hsmv_rpt_nbr
                WHERE sdo_inside(ce1.geocode_pt_3857, MDSYS.SDO_GEOMETRY(
                    2003,  -- 2-dimensional polygon
                    3857,  -- SRID
                    NULL,
                    MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,3), -- one rectangle (1003 = exterior)
                    MDSYS.SDO_ORDINATE_ARRAY(
                        :mapExtentMinX, :mapExtentMinY, -- bottom left corner
                        :mapExtentMaxX, :mapExtentMaxY -- top right corner
                    )
                  )) = 'TRUE'";
            }
            else
            {
                queryText = $@"SELECT
                  id AS eventId,
                  ce1.geocode_pt_3857.sdo_point.x AS x,
                  ce1.geocode_pt_3857.sdo_point.y AS y
                FROM crash_evt ce1
                INNER JOIN ({preparedQuery.queryText}) pq
                    ON pq.hsmv_rpt_nbr = ce1.hsmv_rpt_nbr
                WHERE sdo_inside(ce1.geocode_pt_3857, MDSYS.SDO_GEOMETRY(
                    2003,  -- 2-dimensional polygon
                    3857,  -- SRID
                    NULL,
                    MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,3), -- one rectangle (1003 = exterior)
                    MDSYS.SDO_ORDINATE_ARRAY(
                        :mapExtentMinX, :mapExtentMinY, -- bottom left corner
                        :mapExtentMaxX, :mapExtentMaxY -- top right corner
                    )
                  )) = 'TRUE'";
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
                  hsmv_rpt_nbr
                FROM crash_evt ce0";

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
            var whereClause = "key_crash_dt BETWEEN :startDate AND :endDate";

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
            var whereClause = "crash_d IN :dayOfWeek";

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
                AND crash_hh24mi BETWEEN :startTime AND :endTime
            ) OR (
                :startTime > :endTime -- crosses midnight boundary
                AND ( crash_hh24mi >= :startTime OR crash_hh24mi <= :endTime )
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
            var whereClause = @"cnty_cd IN :dotDistrictCountyCodes
                OR gc_cnty_cd IN :dotDistrictCountyCodes";

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
            var whereClause = @"cnty_cd IN :mpoCountyCodes
                OR gc_cnty_cd IN :mpoCountyCodes
                OR EXISTS (
                    SELECT NULL FROM st
                    WHERE st.link_id = ce0.crash_seg_id
                    AND st.mpo_bnd_id IN :partialCountyMpoIds
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
            // TODO: redesign inputs to make "unincorporated" option explicit

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
            var whereClause = @"cnty_cd IN :fullCountyCodes
                OR gc_cnty_cd IN :fullCountyCodes
                OR key_geography IN :unincorporatedCountyCodes
                OR gc_key_geography IN :unincorporatedCountyCodes";

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
            var whereClause = @"key_geography IN :cityCodes
                OR gc_key_geography IN :cityCodes";

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

            var coordsAsText = customArea.Select(coords => $"{coords.x} {coords.y}");
            var customAreaPolygon = $"POLYGON (({string.Join(", ", coordsAsText)}))";

            // define where clause
            var whereClause = @"sdo_inside(ce0.geocode_pt_3857, sdo_geometry(:customAreaPolygon, 3857)) = 'TRUE'";

            // define oracle parameters
            var parameters = new {
                customAreaPolygon
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
            var whereClause = @"sdo_inside(ce0.geocode_pt_3857, MDSYS.SDO_GEOMETRY(
                    2003,  -- 2-dimensional polygon
                    3857,  -- SRID
                    NULL,
                    MDSYS.SDO_ELEM_INFO_ARRAY(1,1003,3), -- one rectangle (1003 = exterior)
                    MDSYS.SDO_ORDINATE_ARRAY(
                        :customExtentMinX, :customExtentMinY, -- bottom left corner
                        :customExtentMaxX, :customExtentMaxY -- top right corner
                    )
                  )) = 'TRUE'";

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
            var whereClause = @"NEAREST_INTRSECT_ID = :intersectionId
                AND NEAREST_INTRSECT_OFFSET_FT <= :intersectionOffsetFeet
                AND (:matchAnyIntersectionOffsetDir = 1 OR OFFSET_DIR IN :intersectionOffsetDirs)";

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
            var whereClause = @"CRASH_SEG_ID IN :streetLinkIds
                OR (
                  :matchCrossStreets = 1
                  AND NEAREST_INTRSECT_OFFSET_FT <= 100
                  AND NEAREST_INTRSECT_ID IN (
                    SELECT DISTINCT INTERSECTION_ID
                    FROM INTRSECT_NODE
                    WHERE NODE_ID IN (
                        SELECT ST.REF_IN_ID
                        FROM ST
                        WHERE ST.LINK_ID IN :streetLinkIds
                        UNION
                        SELECT ST.NREF_IN_ID
                        FROM ST
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
            var whereClause = "CRASH_SEG_ID IN :customNetworkLinkIds";

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
            var whereClause = "is_public_rd = 'Y'";

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
            var whereClause = "FORM_TYPE_CD IN :formType";

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
            var whereClause = "CODEABLE = 'Y'";

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

            var agencyIds = reportingAgency
                .Where(agencyId => agencyId == 1 || agencyId > 14)
                .ToList();
            var fhpTroops = reportingAgency
                .Where(agencyId => agencyId > 1 && agencyId <= 14)
                .ToList();

            // define where clause
            var whereClause = @"KEY_RPTG_AGNCY IN :agencyIds
                OR (KEY_RPTG_AGNCY = 1 AND KEY_RPTG_UNIT IN :fhpTroops)";

            // define oracle parameters
            var parameters = new {
                agencyIds = agencyIds.Any() ? agencyIds : _PSEUDO_EMPTY_INT_LIST,
                fhpTroops = fhpTroops.Any() ? fhpTroops : _PSEUDO_EMPTY_INT_LIST
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
            var whereClause = @"EXISTS (
                SELECT NULL FROM DRIVER
                WHERE ce0.HSMV_RPT_NBR = DRIVER.HSMV_RPT_NBR
                AND DRIVER.KEY_GENDER IN :driverGender
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
            var whereClause = @"(:matchUnknownDriverAge = 1
                AND NOT EXISTS (
                    SELECT NULL FROM DRIVER
                    WHERE ce0.HSMV_RPT_NBR = DRIVER.HSMV_RPT_NBR
                )
            ) OR EXISTS (
                SELECT NULL FROM DRIVER
                WHERE ce0.HSMV_RPT_NBR = DRIVER.HSMV_RPT_NBR
                AND (
                    AGE_RNG IN :driverAgeRanges
                    OR (:matchUnknownDriverAge = 1 AND AGE_RNG IS NULL)
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
            var whereClause = @"EXISTS (
                SELECT NULL FROM NON_MOTORIST
                WHERE NON_MOTORIST.KEY_DESC IN (230, 231)
                AND ce0.HSMV_RPT_NBR = NON_MOTORIST.HSMV_RPT_NBR
                AND (
                    AGE_RNG IN :pedestrianAgeRanges
                    OR (:matchUnknownPedestrianAge = 1 AND AGE_RNG IS NULL)
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
            var whereClause = @"EXISTS (
                SELECT NULL FROM NON_MOTORIST
                WHERE NON_MOTORIST.KEY_DESC IN (232, 233)
                AND ce0.HSMV_RPT_NBR = NON_MOTORIST.HSMV_RPT_NBR
                AND (
                  AGE_RNG IN :cyclistAgeRanges
                  OR (:matchUnknownCyclistAge = 1 AND AGE_RNG IS NULL)
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
            var whereClause = @"(:matchNonAutoModePed = 1 AND PED_CNT > 0)
                OR (:matchNonAutoModeBike = 1 AND BIKE_CNT > 0)
                OR (:matchNonAutoModeMoped = 1 AND MOPED_CNT > 0)
                OR (:matchNonAutoModeMotorcycle = 1 AND MOTORCYCLE_CNT > 0)";

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
            var whereClause = @"(:matchTransportEms = 1 AND TRANS_BY_EMS_CNT > 0)
                OR (:matchTransportLawEnforcement = 1 AND TRANS_BY_LE_CNT > 0)
                OR (:matchTransportOther = 1 AND TRANS_BY_OTH_CNT > 0)";

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
            var whereClause = @"(:matchBehavioralAlcohol = 1 AND IS_ALC_REL = 'Y')
                OR (:matchBehavioralDrugs = 1 AND IS_DRUG_REL = 'Y')
                OR (:matchBehavioralDistraction = 1 AND IS_DISTRACTED = 'Y')
                OR (:matchBehavioralAggressive = 1 AND IS_AGGRESSIVE = 'Y')";

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
            // TODO: if this filter should remain, tag crashes with common violation types in etl

            // test for valid filter
            if (commonViolations == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"EXISTS (
              SELECT NULL FROM VIOLATION
              WHERE ce0.HSMV_RPT_NBR = VIOLATION.HSMV_RPT_NBR
              AND (
                (:matchViolationSpeed = 1 AND UPPER(VIOLATION.CHARGE) LIKE '%SPEED%')
                OR (
                  :matchViolationRightOfWay = 1
                  AND (
                    UPPER(VIOLATION.CHARGE) LIKE '%FAIL% TO YIELD%'
                    OR UPPER(VIOLATION.CHARGE) LIKE '%RIGHT OF WAY%'
                  )
                )
                OR (:matchViolationTrafficControl = 1 AND UPPER(VIOLATION.CHARGE) LIKE '%TRAFFIC CONTROL%')
                OR (:matchViolationCarelessDriving = 1 AND UPPER(VIOLATION.CHARGE) LIKE '%CARELESS%')
                OR (
                  :matchViolationDui = 1
                  AND (
                    REGEXP_LIKE(VIOLATION.FL_STATUTE_NBR, '^316(\.|-|\,| |)193(\.|-|\,| |\(|$)')
                    OR (
                      REGEXP_LIKE(UPPER(VIOLATION.FL_STATUTE_NBR), '^(|CHAPTER )316$')
                      AND REGEXP_LIKE(UPPER(VIOLATION.CHARGE), '^D(|\.| )U(|\.| )I')
                    )
                  )
                )
                OR (:matchViolationRedLight = 1 AND UPPER(VIOLATION.FL_STATUTE_NBR) LIKE '316%075%1%C%')
              )
            )
            OR (
              :matchViolationRedLight = 1
              AND EXISTS (
                SELECT NULL FROM DRIVER FD
                WHERE ce0.HSMV_RPT_NBR = FD.HSMV_RPT_NBR
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
            var whereClause = @"EXISTS (
                SELECT NULL FROM VEH
                WHERE ce0.HSMV_RPT_NBR = VEH.HSMV_RPT_NBR
                AND VEH.KEY_VEH_TYPE IN :vehicleTypes
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
            var whereClause = "crash_type_simplified = :crashTypeSimple";

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
            var whereClause = "key_crash_type = :crashTypeDetailed";

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
            var whereClause = @"KEY_BIKE_PED_CRASH_TYPE IN :bikePedCrashType
            OR (
                :matchBikePedUntyped = 1
                AND (
                    PED_CNT > 0
                    OR BIKE_CNT > 0
                    OR FIRST_HE IN ('Pedestrian', 'Pedalcycle')
                )
                AND KEY_BIKE_PED_CRASH_TYPE IS NULL
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
            var whereClause = @"EXISTS (
                SELECT NULL FROM VEH
                WHERE ce0.HSMV_RPT_NBR = VEH.HSMV_RPT_NBR
                AND VEH.KEY_CMV_CONFIG IN :cmvConfiguration
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
            var whereClause = @"KEY_CONTRIB_CIRCUM_ENV1 IN :environmentalCircumstance
                OR KEY_CONTRIB_CIRCUM_ENV2 IN :environmentalCircumstance
                OR KEY_CONTRIB_CIRCUM_ENV3 IN :environmentalCircumstance";

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
            var whereClause = @"KEY_CONTRIB_CIRCUM_RD1 IN :roadCircumstance
                OR KEY_CONTRIB_CIRCUM_RD2 IN :roadCircumstance
                OR KEY_CONTRIB_CIRCUM_RD3 IN :roadCircumstance";

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
            var whereClause = "KEY_FIRST_HE IN :firstHarmfulEvent";

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
            var whereClause = @"KEY_LIGHT_COND IN :lightCondition";

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
            var whereClause = @"KEY_RD_SYS_ID IN :roadSystemIdentifier";

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
            var whereClause = @"KEY_WEATHER_COND IN :weatherCondition";

            // define oracle parameters
            var parameters = new { weatherCondition };

            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateLaneDeparturePredicate(LaneDepartures laneDepartures)
        {
            // TODO: if this filter should remain, tag crashes with lane departure types in etl

            // test for valid filter
            if (laneDepartures == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:matchLaneDepartureOffRoad = 1 AND KEY_CRASH_TYPE = 45)
                OR (
                    :matchLaneDepartureRollover = 1
                    AND KEY_CRASH_TYPE = 45
                    AND (
                        EXISTS (
                            SELECT NULL FROM VEH
                            WHERE ce0.HSMV_RPT_NBR = VEH.HSMV_RPT_NBR
                            AND (
                                VEH.KEY_MOST_HE = 1
                                OR VEH.KEY_HE1 = 1
                                OR VEH.KEY_HE2 = 1
                                OR VEH.KEY_HE3 = 1
                                OR VEH.KEY_HE4 = 1
                            )
                        )
                    )
                )
                OR (
                    :matchLaneDepartureCollision = 1
                    AND KEY_CRASH_TYPE = 45
                    AND (
                        EXISTS (
                            SELECT NULL FROM VEH
                            WHERE ce0.HSMV_RPT_NBR = VEH.HSMV_RPT_NBR
                            AND (
                                VEH.KEY_MOST_HE IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR VEH.KEY_HE1 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR VEH.KEY_HE2 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR VEH.KEY_HE3 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                                OR VEH.KEY_HE4 IN (40,42,43,44,45,46,47,48,49,50,51,52,53,54,55,56,57,58,59,60)
                            )
                        )
                    )
                )
                OR (:matchLaneDepartureOncoming = 1 AND KEY_CRASH_TYPE = 41)
                OR (:matchLaneDepartureSideswipe = 1 AND KEY_CRASH_TYPE IN (46,55))";

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
            // TODO: if this filter should remain, tag crashes with hit and run in etl

            // test for valid filter
            if (otherCircumstances == null)
            {
                return (null, null);
            }

            // define where clause
            var whereClause = @"(:matchSchoolBusRelated = 1 AND IS_SCH_BUS_REL = 'Y')
                OR (:matchWithinCityLimits = 1 AND IS_WITHIN_CITY_LIM = 'Y')
                OR (:matchWithinInterchange = 1 AND IS_1ST_HE_WITHIN_INTRCHG = 'Y')
                OR (:matchWorkZoneRelated = 1 AND IS_WORK_ZN_REL = 'Y')
                OR (:matchWorkersInWorkZone = 1 AND IS_WORKERS_IN_WORK_ZN = 'Y')
                OR (:matchLawEnforcementInWorkZone = 1 AND IS_LE_IN_WORK_ZN = 'Y')
                OR (
                  :matchHitAndRun = 1
                  AND EXISTS (
                    SELECT NULL FROM VEH
                    WHERE ce0.HSMV_RPT_NBR = VEH.HSMV_RPT_NBR
                    AND VEH.IS_HIT_AND_RUN = 'Y'
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
