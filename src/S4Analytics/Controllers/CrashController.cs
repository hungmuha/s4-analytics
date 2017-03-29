using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System.Collections.Generic;
using System.Linq;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    public class CrashController : S4Controller
    {
        private ICrashRepository _crashRepo;

        public CrashController(ICrashRepository repo)
        {
            _crashRepo = repo;
        }

        [HttpGet("query/{queryId}", Name = "GetQuery")]
        public IActionResult GetQuery(int queryId)
        {
            var queryExists = _crashRepo.QueryExists(queryId);
            if (!queryExists)
            {
                return NotFound();
            }
            return new ObjectResult(new { queryId });
        }

        [HttpPost("query")]
        public IActionResult CreateQuery([FromBody] CrashQuery query)
        {
            var queryId = _crashRepo.CreateQuery(query);
            return CreatedAtRoute("GetQuery", new { queryId }, query);
        }

        [HttpPost("query-test")]
        public IActionResult CreateQueryTest([FromBody] CrashQuery query)
        {
            (var queryText, var parameters) = _crashRepo.CreateQueryTest(query);
            return Content(queryText + "\r\n" + parameters.DumpText());
        }

        [HttpGet("{queryId}")]
        public IActionResult GetCrashes(int queryId)
        {
            var queryExists = _crashRepo.QueryExists(queryId);
            if (!queryExists)
            {
                return NotFound();
            }
            var results = _crashRepo.GetCrashes(queryId);
            var data = AjaxSafeData(results);
            return new ObjectResult(data);
        }
    }
}
