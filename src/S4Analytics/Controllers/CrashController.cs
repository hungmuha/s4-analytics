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
        private CrashRepository _crashRepo;

        public CrashController(CrashRepository repo)
        {
            _crashRepo = repo;
        }

        [HttpPost("query")]
        public IActionResult CreateQuery([FromBody] CrashQuery query)
        {
            var crashQueryRef = _crashRepo.CreateQuery(query);
            return CreatedAtRoute("GetCrashes", new { crashQueryRef.queryToken }, crashQueryRef);
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
    }
}
