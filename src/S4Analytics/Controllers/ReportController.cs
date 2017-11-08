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

        [HttpGet("crash/year")]
        public IActionResult GetCrashCountsByYear()
        {
            var results = _reportRepo.GetCrashCountsByYear();
            return new ObjectResult(results);
        }

        [HttpGet("crash/{year}/month")]
        public IActionResult GetCrashCountsByMonth(int year, bool yearOnYear)
        {
            var results = _reportRepo.GetCrashCountsByMonth(year, yearOnYear);
            return new ObjectResult(results);
        }

        [HttpGet("crash/{year}/day")]
        public IActionResult GetCrashCountsByDay(int year, bool yearOnYear, bool alignByWeek)
        {
            var results = _reportRepo.GetCrashCountsByDay(year, yearOnYear, alignByWeek);
            return new ObjectResult(results);
        }
    }
}
