using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/reporting")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CitationReportingController : Controller
    {
        private CitationReportingRepository _reportRepo;

        public CitationReportingController(CitationReportingRepository repo)
        {
            _reportRepo = repo;
        }

        [HttpPost("citation/year")]
        public IActionResult GetCitationCountsByYear([FromBody] CitationsOverTimeQuery query)
        {
            var results = _reportRepo.GetCitationCountsByYear(query);
            return new ObjectResult(results);
        }

        [HttpPost("citation/{year}/month")]
        public IActionResult GetCitationCountsByMonth(int year, [FromBody] CitationsOverTimeQuery query)
        {
            var results = _reportRepo.GetCitationCountsByMonth(year, query);
            return new ObjectResult(results);
        }

        [HttpPost("citation/{year}/day")]
        public IActionResult GetCitationCountsByDay(int year, bool alignByWeek, [FromBody] CitationsOverTimeQuery query)
        {
            var results = _reportRepo.GetCitationCountsByDay(year, alignByWeek, query);
            return new ObjectResult(results);
        }

        [HttpGet("citation/geographies")]
        public IActionResult GetGeographyLookups()
        {
            var results = _reportRepo.GetGeographyLookups();
            return new ObjectResult(results);
        }

        [HttpGet("citation/agencies")]
        public IActionResult GetAgencyLookups()
        {
            var results = _reportRepo.GetAgencyLookups();
            return new ObjectResult(results);
        }
    }
}
