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
