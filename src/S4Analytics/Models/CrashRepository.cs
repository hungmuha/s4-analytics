using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;

namespace S4Analytics.Models
{
    public class CrashRepository : ICrashRepository
    {
        private string _connStr;

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
                  ON fact_crash_evt.key_1st_he = dim_harmful_evt.ID
                WHERE ";

            var whereClauses = new List<string>();
            var dynamicParameters = new DynamicParameters(new { queryId });
            var generators = new List<Func<CrashQuery, (string whereClause, object parameters)>>();

            generators.Add(GenerateDateRangePredicate);
            generators.Add(GenerateDayOfWeekPredicate);

            generators.ForEach(generator => {
                (var whereClause, object parameters) = generator.Invoke(query);
                if (whereClause != null)
                {
                    whereClauses.Add(whereClause);
                    dynamicParameters.AddDynamicParams(parameters);
                }
            });

            queryText += "(" + string.Join(") AND (", whereClauses) + ")";

            return (queryText, dynamicParameters);
        }

        private (string whereClause, object parameters) GenerateDateRangePredicate(CrashQuery query)
        {
            var whereClause = "FACT_CRASH_EVT.KEY_CRASH_DT BETWEEN :startDate AND :endDate";
            var parameters = new {
                startDate = new DateTime(query.dateRange.startDate.Year, query.dateRange.startDate.Month, query.dateRange.startDate.Day, 0, 0, 0, 0),
                endDate = new DateTime(query.dateRange.endDate.Year, query.dateRange.endDate.Month, query.dateRange.endDate.Day, 23, 59, 59, 999)
            };
            return (whereClause, parameters);
        }

        private (string whereClause, object parameters) GenerateDayOfWeekPredicate(CrashQuery query)
        {
            if (query.dayOfWeek != null && query.dayOfWeek.AsList().Count > 0)
            {
                var whereClause = "TO_CHAR(FACT_CRASH_EVT.KEY_CRASH_DT, 'D') IN :dayOfWeek";
                var parameters = new { query.dayOfWeek };
                return (whereClause, parameters);
            }
            return (null, null);
        }
    }
}
