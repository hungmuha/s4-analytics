using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class LookupName : IEquatable<LookupName>
    {
        public string name;

        #region IEquatable implementation

        public override bool Equals(object obj)
        {
            return Equals(obj as LookupName);
        }

        public bool Equals(LookupName ln)
        {
            // if parameter is null, return false
            if (ln is null) { return false; }

            // optimization for a common success case
            if (ReferenceEquals(this, ln)) { return true; }

            // if run-time types are not exactly the same, return false
            if (GetType() != ln.GetType()) { return false; }

            // return true if the name field matches
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality
            return (name == ln.name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LookupName lhs, LookupName rhs)
        {
            if (lhs is null)
            {
                return (rhs is null) ? true : false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(LookupName lhs, LookupName rhs) => !(lhs == rhs);

        #endregion

    }

    public class LookupKeyAndName : IEquatable<LookupKeyAndName>
    {
        public int key;
        public string name;

        #region IEquatable implementation

        public override bool Equals(object obj)
        {
            return Equals(obj as LookupName);
        }

        public bool Equals(LookupKeyAndName ln)
        {
            // if parameter is null, return false
            if (ln is null) { return false; }

            // optimization for a common success case
            if (ReferenceEquals(this, ln)) { return true; }

            // if run-time types are not exactly the same, return false
            if (GetType() != ln.GetType()) { return false; }

            // return true if the key and name fields match
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality
            return (key == ln.key && name == ln.name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LookupKeyAndName lhs, LookupKeyAndName rhs)
        {
            if (lhs is null)
            {
                return (rhs is null) ? true : false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(LookupKeyAndName lhs, LookupKeyAndName rhs) => !(lhs == rhs);

        #endregion

    }

    public class LookupKeyCodeAndName : IEquatable<LookupKeyCodeAndName>
    {
        public int key;
        public int code;
        public string name;

        #region IEquatable implementation

        public override bool Equals(object obj)
        {
            return Equals(obj as LookupName);
        }

        public bool Equals(LookupKeyCodeAndName ln)
        {
            // if parameter is null, return false
            if (ln is null) { return false; }

            // optimization for a common success case
            if (ReferenceEquals(this, ln)) { return true; }

            // if run-time types are not exactly the same, return false
            if (GetType() != ln.GetType()) { return false; }

            // return true if the key, code and name fields match
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality
            return (key == ln.key && code == ln.code && name == ln.name);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(LookupKeyCodeAndName lhs, LookupKeyCodeAndName rhs)
        {
            if (lhs is null)
            {
                return (rhs is null) ? true : false;
            }
            return lhs.Equals(rhs);
        }

        public static bool operator !=(LookupKeyCodeAndName lhs, LookupKeyCodeAndName rhs) => !(lhs == rhs);

        #endregion

    }

    public class LookupRepository
    {
        protected readonly string _connStr;
        protected static readonly string[] _behavioralFactors = { "Aggressive Driving", "Distracted Driving", "Alcohol Involved", "Drugs Involved" };
        protected static readonly string[] _databases = { "All Crashes", "Crashes on Public Roads", "Long Forms on Public Roads" };
        protected static readonly string[] _dayOrNight = { "Day", "Night" };
        protected static readonly string[] _formTypes = { "Long", "Short" };
        protected static readonly string[] _noYes = { "No", "Yes" };

        public LookupRepository(IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.FlatConnStr;
        }

        public IEnumerable<LookupName> GetBehavioralFactorsLookups()
        {
            return GetLookupNames(_behavioralFactors);
        }

        public IEnumerable<LookupName> GetDatabaseLookups()
        {
            return GetLookupNames(_databases);
        }

        public IEnumerable<LookupKeyAndName> GetCountyLookups()
        {
            var queryText = @"SELECT cnty_cd AS key, cnty_nm AS name
                FROM dim_geography
                WHERE city_cd = 0
                AND cnty_cd <> 68
                ORDER BY cnty_nm";
            IEnumerable<LookupKeyAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyAndName>(queryText, new { });
            }
            return results;
        }

        public IEnumerable<LookupKeyAndName> GetCityLookups()
        {
            var queryText = @"SELECT id AS key, city_nm AS name
                FROM dim_geography
                WHERE city_cd <> 0
                AND cnty_cd <> 68
                ORDER BY city_nm";
            IEnumerable<LookupKeyAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyAndName>(queryText, new { });
            }
            return results;
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
            // TODO: WHERE obsolete_cd = 'N'?
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

        public IEnumerable<LookupName> GetFormTypeLookups()
        {
            return GetLookupNames(_formTypes);
        }

        public IEnumerable<LookupName> GetCodeableLookups()
        {
            return GetLookupNames(_noYes);
        }

        public IEnumerable<LookupName> GetCmvInvolvedLookups()
        {
            return GetLookupNames(_noYes);
        }

        public IEnumerable<LookupName> GetBikeInvolvedLookups()
        {
            return GetLookupNames(_noYes);
        }

        public IEnumerable<LookupName> GetPedInvolvedLookups()
        {
            return GetLookupNames(_noYes);
        }

        public IEnumerable<LookupKeyCodeAndName> GetCrashSeverityLookups()
        {
            var queryText = @"SELECT id AS key, crash_attr_cd AS code, crash_attr_tx AS name
                FROM v_crash_sev_dtl
                ORDER BY crash_attr_cd";
            IEnumerable<LookupKeyCodeAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyCodeAndName>(queryText, new { });
            }
            return results;
        }

        public IEnumerable<LookupKeyAndName> GetCrashTypeLookups()
        {
            var queryText = @"SELECT id AS key, crash_attr_tx AS name
                FROM v_crash_type
                ORDER BY crash_attr_tx";
            IEnumerable<LookupKeyAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyAndName>(queryText, new { });
            }
            return results;
        }

        public IEnumerable<LookupKeyCodeAndName> GetRoadSysIdLookups()
        {
            var queryText = @"SELECT id AS key, crash_attr_cd AS code, crash_attr_tx AS name
                FROM v_crash_road_sys_id
                ORDER BY crash_attr_cd";
            IEnumerable<LookupKeyCodeAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyCodeAndName>(queryText, new { });
            }
            return results;
        }

        public IEnumerable<LookupName> GetIntersectionRelatedLookups()
        {
            return GetLookupNames(_noYes);
        }

        public IEnumerable<LookupName> GetDayOrNightLookups()
        {
            return GetLookupNames(_dayOrNight);
        }

        public IEnumerable<LookupName> GetLaneDepartureLookups()
        {
            return GetLookupNames(_noYes);
        }

        public IEnumerable<LookupKeyCodeAndName> GetWeatherConditionLookups()
        {
            var queryText = @"SELECT id AS key, crash_attr_cd AS code, crash_attr_tx AS name
                FROM v_crash_weather_cond
                ORDER BY crash_attr_cd";
            IEnumerable<LookupKeyCodeAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyCodeAndName>(queryText, new { });
            }
            return results;
        }

        public IEnumerable<LookupName> GetDotDistrictLookups()
        {
            var queryText = @"SELECT DISTINCT dot_district_nm AS name
                FROM dim_geography WHERE dot_district_nm IS NOT NULL
                ORDER BY dot_district_nm";
            IEnumerable<LookupName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupName>(queryText, new { });
            }
            return results;
        }

        //public IEnumerable<LookupName> GetMpoTpoLookups()
        //{
        //    var queryText = @"SELECT DISTINCT mpo_nm AS name
        //        FROM dim_geography
        //        ORDER BY mpo_nm";
        //    IEnumerable<LookupName> results;
        //    using (var conn = new OracleConnection(_connStr))
        //    {
        //        results = conn.Query<LookupName>(queryText, new { });
        //    }
        //    return results;
        //}

        //public IEnumerable<LookupKeyCodeAndName> GetDriverGenderLookups()
        //{
        //    var queryText = @"SELECT id AS key, driver_attr_cd AS code, driver_attr_tx AS name
        //        FROM v_driver_gender
        //        ORDER BY id";
        //    IEnumerable<LookupKeyCodeAndName> results;
        //    using (var conn = new OracleConnection(_connStr))
        //    {
        //        results = conn.Query<LookupKeyCodeAndName>(queryText, new { });
        //    }
        //    return results;
        //}

        public IEnumerable<LookupName> GetDriverAgeRangeLookups()
        {
            var queryText = @"SELECT id AS key, driver_attr_cd AS code, driver_attr_tx AS name
                FROM v_driver_age_rng
                ORDER BY driver_attr_cd";
            IEnumerable<LookupKeyCodeAndName> results;
            using (var conn = new OracleConnection(_connStr))
            {
                results = conn.Query<LookupKeyCodeAndName>(queryText, new { });
            }
            return DistinctLookupNames(results);
        }

        //public IEnumerable<LookupName> GetNonMotoristAgeRangeLookups()
        //{
        //    var queryText = @"SELECT id AS key, nm_attr_cd AS code, nm_attr_tx AS name
        //        FROM v_nm_age_rng
        //        ORDER BY nm_attr_cd";
        //    IEnumerable<LookupKeyCodeAndName> results;
        //    using (var conn = new OracleConnection(_connStr))
        //    {
        //        results = conn.Query<LookupKeyCodeAndName>(queryText, new { });
        //    }
        //    return DistinctLookupNames(results);
        //}

        //public IEnumerable<LookupName> GetCyclistAgeRangeLookups()
        //{
        //    return GetNonMotoristAgeRangeLookups();
        //}

        //public IEnumerable<LookupName> GetPedestrianAgeRangeLookups()
        //{
        //    return GetNonMotoristAgeRangeLookups();
        //}

        //public IEnumerable<LookupName> GetCrashTypeSimplified()
        //{
        //    var queryText = @"SELECT DISTINCT crash_attr_tx AS name
        //        FROM v_crash_type_simplified
        //        ORDER BY crash_attr_tx";
        //    IEnumerable<LookupName> results;
        //    using (var conn = new OracleConnection(_connStr))
        //    {
        //        results = conn.Query<LookupName>(queryText, new { });
        //    }
        //    return results;
        //}

        //public IEnumerable<LookupName> GetCrashTypeDetailed()
        //{
        //    var queryText = @"SELECT DISTINCT crash_attr_tx AS name
        //        FROM v_crash_type
        //        ORDER BY crash_attr_tx";
        //    IEnumerable<LookupName> results;
        //    using (var conn = new OracleConnection(_connStr))
        //    {
        //        results = conn.Query<LookupName>(queryText, new { });
        //    }
        //    return results;
        //}

        /// <summary>
        /// Utility method to convert an array of strings into an IEnumerable of LookupName.
        /// </summary>
        /// <param name="names">An array of strings.</param>
        /// <returns>An IEnumerable of LookupName.</returns>
        private IEnumerable<LookupName> GetLookupNames(string[] names)
        {
            var arr = new LookupName[names.Length];
            for(var i = 0; i < names.Length; i++)
            {
                arr[i] = new LookupName() { name = names[i] };
            }
            return arr;
        }

        /// <summary>
        /// Utility method to extract an IEnumerable of distinct LookupName from an
        /// IEnumerable of LookupKeyAndName.
        /// </summary>
        /// <param name="origList">An IEnumerable of LookupKeyAndName.</param>
        /// <returns>An IEnumerable of distinct LookupName.</returns>
        private IEnumerable<LookupName> DistinctLookupNames(IEnumerable<LookupKeyAndName> origList)
        {
            var distinctList = new List<LookupName>();
            foreach (var origItem in origList)
            {
                var lookupName = new LookupName { name = origItem.name };
                if (!distinctList.Contains(lookupName)) { distinctList.Add(lookupName); }
            }
            return distinctList;
        }

        /// <summary>
        /// Utility method to extract an IEnumerable of distinct LookupName from an
        /// IEnumerable of LookupKeyCodeAndName.
        /// </summary>
        /// <param name="origList">An IEnumerable of LookupKeyCodeAndName.</param>
        /// <returns>An IEnumerable of distinct LookupName.</returns>
        private IEnumerable<LookupName> DistinctLookupNames(IEnumerable<LookupKeyCodeAndName> origList)
        {
            var distinctList = new List<LookupName>();
            foreach (var origItem in origList)
            {
                var lookupName = new LookupName { name = origItem.name };
                if (!distinctList.Contains(lookupName)) { distinctList.Add(lookupName); }
            }
            return distinctList;
        }

    }
}
