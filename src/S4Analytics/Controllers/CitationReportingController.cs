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

        [HttpGet("citation/max-event-year")]
        public IActionResult GetMaxEventYear()
        {
            var maxEventYear = _reportRepo.GetMaxEventYear();
            return new ObjectResult(maxEventYear);
        }

        [HttpGet("citation/max-load-year")]
        public IActionResult GetMaxLoadYear()
        {
            var maxLoadYear = _reportRepo.GetMaxLoadYear();
            return new ObjectResult(maxLoadYear);
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

        [HttpPost("citation/{year}/{attrName}")]
        public IActionResult GetCitationCountsByAttribute(int year, string attrName, [FromBody] CitationsOverTimeQuery query)
        {
            /* attrName can be one of the following:
               violation-type,
               violator-age,
               violator-gender*/
            var results = _reportRepo.GetCitationCountsByAttribute(year, attrName, query);
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
