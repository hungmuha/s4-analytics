using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ReportController : Controller
    {
        private ReportRepository _reportRepo;

        public ReportController(ReportRepository repo)
        {
            _reportRepo = repo;
        }

        [HttpPost("crash/year")]
        public IActionResult GetCrashCountsByYear([FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetCrashCountsByYear(query);
            return new ObjectResult(results);
        }

        [HttpPost("crash/{year}/month")]
        public IActionResult GetCrashCountsByMonth(int year, [FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetCrashCountsByMonth(year, query);
            return new ObjectResult(results);
        }

        [HttpPost("crash/{year}/day")]
        public IActionResult GetCrashCountsByDay(int year, bool alignByWeek, [FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetCrashCountsByDay(year, alignByWeek, query);
            return new ObjectResult(results);
        }

        [HttpGet("geographies")]
        public IActionResult GetGeographyLookups()
        {
            var results = _reportRepo.GetGeographyLookups();
            return new ObjectResult(results);
        }

        [HttpGet("agencies")]
        public IActionResult GetAgencyLookups()
        {
            var results = _reportRepo.GetAgencyLookups();
            return new ObjectResult(results);
        }
    }
}
