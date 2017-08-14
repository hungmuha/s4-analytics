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
            _connStr = serverOptions.Value.WarehouseConnStr;
            _conn = new OracleConnection(_connStr);
        }

        public string GetUserNameFromToken(Guid token)
        {
            var cmdText = "SELECT user_nm FROM v_html5_conduit WHERE token = :token";
            var queryParams = new OracleDynamicParameters();
            queryParams.Add("token", OracleDbType.Raw, System.Data.ParameterDirection.Input, token);
            var userName = _conn.QueryFirstOrDefault<string>(cmdText, queryParams);
            return userName;
        }
    }
}
