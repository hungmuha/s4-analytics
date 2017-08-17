using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CrashController : Controller
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
            return Content(queryText + "\r\n\r\n" + parameters.ToPrettyJson());
        }

        [HttpPost("query")]
        public IActionResult CreateQuery([FromBody] CrashQuery query)
        {
            var queryToken = _crashRepo.CreateQuery(query);
            return CreatedAtRoute("GetCrashes", new { queryToken }, query);
        }

        [HttpGet("{queryToken}", Name = "GetCrashes")]
        public IActionResult GetCrashes(string queryToken, int fromIndex, int toIndex)
        {
            var queryExists = _crashRepo.QueryExists(queryToken);
            var badIndices = fromIndex < 0 || toIndex < 0 || toIndex < fromIndex;

            if (!queryExists) { return NotFound(); }
            if (badIndices) { return BadRequest(); }

            var results = _crashRepo.GetCrashes(queryToken, fromIndex, toIndex);
            return new ObjectResult(results);
        }

        [HttpGet("{queryToken}/feature")]
        public IActionResult GetCrashFeatures(
            string queryToken,
            [FromQuery] double minX,
            [FromQuery] double minY,
            [FromQuery] double maxX,
            [FromQuery] double maxY)
        {
            var queryExists = _crashRepo.QueryExists(queryToken);
            var extent = new Extent(minX, minY, maxX, maxY);

            if (!queryExists) { return NotFound(); }
            if (!extent.IsValid) { return BadRequest(); }

            var results = _crashRepo.GetCrashFeatureCollection(queryToken, extent);
            return new ObjectResult(results);
        }

        // TODO: parameterize the specific attribute(s) to summarize
        [HttpGet("{queryToken}/summary/crash-severity")]
        public IActionResult GetCrashSeveritySummary(string queryToken)
        {
            var queryExists = _crashRepo.QueryExists(queryToken);
            if (!queryExists)
            {
                return NotFound();
            }
            var results = _crashRepo.GetCrashSeveritySummary(queryToken);
            return new ObjectResult(results);
        }
    }
}
