using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/reporting/crash")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CrashReportingController : Controller
    {
        private CrashReportingRepository _reportRepo;

        public CrashReportingController(CrashReportingRepository repo)
        {
            _reportRepo = repo;
        }

        [HttpGet("max-event-year")]
        public IActionResult GetMaxEventYear()
        {
            var maxEventYear = _reportRepo.GetMaxEventYear();
            return new ObjectResult(maxEventYear);
        }

        [HttpGet("max-load-year")]
        public IActionResult GetMaxLoadYear()
        {
            var maxLoadYear = _reportRepo.GetMaxLoadYear();
            return new ObjectResult(maxLoadYear);
        }

        [HttpPost("year")]
        public IActionResult GetCrashCountsByYear([FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetCrashCountsByYear(query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/month")]
        public IActionResult GetCrashCountsByMonth(int year, [FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetCrashCountsByMonth(year, query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/day")]
        public IActionResult GetCrashCountsByDay(int year, bool alignByWeek, [FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetCrashCountsByDay(year, alignByWeek, query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/{attrName}")]
        public IActionResult GetCrashCountsByAttribute(int year, string attrName, [FromBody] CrashesOverTimeQuery query)
        {
            /* attrName can be one of the following:
               day-of-week,
               hour-of-day,
               weather-condition,
               light-condition,
               crash-type,
               crash-severity,
               road-surface-condition,
               first-harmful-event */
            var results = _reportRepo.GetCrashCountsByAttribute(year, attrName, query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/timeliness")]
        public IActionResult GetTimelinessCrashCountsByMonth(int year, [FromBody] CrashesOverTimeQuery query)
        {
            var results = _reportRepo.GetTimelinessCrashCountsByMonth(year, query);
            return new ObjectResult(results);
        }
    }
}
