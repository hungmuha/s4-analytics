using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/reporting/citation")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CitationReportingController : Controller
    {
        private CitationReportingRepository _reportRepo;

        public CitationReportingController(CitationReportingRepository repo)
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
        public IActionResult GetCitationCountsByYear([FromBody] CitationsOverTimeQuery query)
        {
            var results = _reportRepo.GetCitationCountsByYear(query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/month")]
        public IActionResult GetCitationCountsByMonth(int year, [FromBody] CitationsOverTimeQuery query)
        {
            var results = _reportRepo.GetCitationCountsByMonth(year, query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/day")]
        public IActionResult GetCitationCountsByDay(int year, bool alignByWeek, [FromBody] CitationsOverTimeQuery query)
        {
            var results = _reportRepo.GetCitationCountsByDay(year, alignByWeek, query);
            return new ObjectResult(results);
        }

        [HttpPost("{year}/{attrName}")]
        public IActionResult GetCitationCountsByAttribute(int year, string attrName, [FromBody] CitationsOverTimeQuery query)
        {
            /* attrName can be one of the following:
               violation-type,
               violator-age,
               violator-gender*/
            var results = _reportRepo.GetCitationCountsByAttribute(year, attrName, query);
            return new ObjectResult(results);
        }
    }
}
