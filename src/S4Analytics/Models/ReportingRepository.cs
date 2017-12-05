using Dapper;
using Microsoft.Extensions.Options;
using MoreLinq;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Models
{
    public class LookupKeyAndName
    {
        public int key;
        public string name;
    }

    public class PreparedWhereClause
    {
        public string whereClauseText;
        public Dictionary<string, object> whereClauseParameters;
        public DynamicParameters DynamicParams
        {
            get
            {
                var dynamicParams = new DynamicParameters();
                dynamicParams.AddDict(whereClauseParameters);
                return dynamicParams;
            }
        }
        public PreparedWhereClause(string whereClauseText, Dictionary<string, object> queryParameters)
        {
            this.whereClauseText = whereClauseText;
            this.whereClauseParameters = queryParameters;
        }
    }

    public class ReportingRepository
    {
        protected readonly string _connStr;

        public ReportingRepository(IOptions<ServerOptions> serverOptions)
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

        public PreparedWhereClause PrepareWhereClause(List<Func<(string, object)>> predicateMethods)
        {
            // initialize where clause and query parameter collections
            var whereClauses = new List<string>();
            var queryParameters = new Dictionary<string, object>();

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

            return new PreparedWhereClause(queryText, queryParameters);
        }


        public (string whereClause, object parameters) GenerateGeographyPredicate(int? geographyId)
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

      
    }
}
