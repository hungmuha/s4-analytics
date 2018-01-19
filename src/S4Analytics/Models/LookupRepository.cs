using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class LookupKeyAndName
    {
        public int key;
        public string name;
    }

    public class LookupRepository
    {
        protected readonly string _connStr;

        public LookupRepository(IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.FlatConnStr;
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
    }
}
