using Dapper;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System;

namespace S4Analytics.Models
{
    public class Html5Conduit
    {
        private string _connStr;
        private OracleConnection _conn;

        public Html5Conduit(IOptions<ServerOptions> serverOptions)
        {
            _connStr = serverOptions.Value.IdentityConnStr;
            _conn = new OracleConnection(_connStr);
        }

        public Html5ConduitDetails GetDetailsFromToken(Guid token)
        {
            var cmdText = "SELECT user_nm AS UserName, json_payload AS JsonPayload FROM v_html5_conduit WHERE token = :token";
            var queryParams = new OracleDynamicParameters();
            queryParams.Add("token", OracleDbType.Raw, System.Data.ParameterDirection.Input, token);
            var details = _conn.QueryFirstOrDefault<Html5ConduitDetails>(cmdText, queryParams);
            return details;
        }
    }

    public class Html5ConduitDetails
    {
        public string UserName { get; set; }
        public string JsonPayload { get; set; }
    }
}
