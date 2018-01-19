using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class LookupController : Controller
    {
        LookupRepository _lookupRepo;

        public LookupController(LookupRepository lookupRepo)
        {
            _lookupRepo = lookupRepo;
        }

        [HttpGet("county")]
        public IActionResult GetCountyLookups()
        {
            var results = _lookupRepo.GetCountyLookups();
            return new ObjectResult(results);
        }

        [HttpGet("city")]
        public IActionResult GetCityLookups()
        {
            var results = _lookupRepo.GetCityLookups();
            return new ObjectResult(results);
        }

        [HttpGet("geography")]
        public IActionResult GetGeographyLookups()
        {
            var results = _lookupRepo.GetGeographyLookups();
            return new ObjectResult(results);
        }

        [HttpGet("agency")]
        public IActionResult GetAgencyLookups()
        {
            var results = _lookupRepo.GetAgencyLookups();
            return new ObjectResult(results);
        }
    }
}
