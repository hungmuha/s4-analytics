using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Extensions.Options;
using S4Analytics.Models;
using Oracle.ManagedDataAccess.Client;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class HealthController
    {
        private string _flatConnStr;
        private string _identityConnStr;

        public HealthController(IOptions<ServerOptions> serverOptions)
        {
            _flatConnStr = serverOptions.Value.FlatConnStr;
            _identityConnStr = serverOptions.Value.IdentityConnStr;
        }

        [HttpGet("check")]
        public IActionResult HealthCheck()
        {
            // issue query against both database servers
            var queryText = "SELECT 'OK' from dual";
            using (var conn = new OracleConnection(_flatConnStr))
            {
                conn.QuerySingle<string>(queryText, new { });
            }
            using (var conn = new OracleConnection(_identityConnStr))
            {
                conn.QuerySingle<string>(queryText, new { });
            }

            // if exception wasn't thrown, return OK
            return new ObjectResult("OK");
        }
    }
}
