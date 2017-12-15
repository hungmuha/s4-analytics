using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
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
