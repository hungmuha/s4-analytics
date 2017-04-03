using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

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

        [HttpPost("query-test")]
        public IActionResult CreateQueryTest([FromBody] CrashQuery query)
        {
            (var queryText, var parameters) = _crashRepo.CreateQueryTest(query);
            return Content(queryText + "\r\n" + parameters.DumpText());
        }

        [HttpPost("query")]
        public IActionResult CreateQuery([FromBody] CrashQuery query)
        {
            var queryToken = _crashRepo.CreateQuery(query);
            return CreatedAtRoute("GetCrashes", new { queryToken }, query);
        }

        [HttpGet("{queryToken}", Name = "GetCrashes")]
        public IActionResult GetCrashes(string queryToken)
        {
            var queryExists = _crashRepo.QueryExists(queryToken);
            if (!queryExists)
            {
                return NotFound();
            }
            var results = _crashRepo.GetCrashes(queryToken);
            var data = AjaxSafeData(results);
            return new ObjectResult(data);
        }
    }
}
