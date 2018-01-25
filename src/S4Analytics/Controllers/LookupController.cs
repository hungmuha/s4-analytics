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

        [HttpGet("database")]
        public IActionResult GetDatabaseLookups()
        {
            var results = _lookupRepo.GetDatabaseLookups();
            return new ObjectResult(results);
        }

        [HttpGet("agency")]
        public IActionResult GetAgencyLookups()
        {
            var results = _lookupRepo.GetAgencyLookups();
            return new ObjectResult(results);
        }

        [HttpGet("formType")]
        public IActionResult GetFormTypeLookups()
        {
            var results = _lookupRepo.GetFormTypeLookups();
            return new ObjectResult(results);
        }

        [HttpGet("codeable")]
        public IActionResult GetCodeableLookups()
        {
            var results = _lookupRepo.GetCodeableLookups();
            return new ObjectResult(results);
        }

        [HttpGet("cmvInvolved")]
        public IActionResult GetCmvInvolvedLookups()
        {
            var results = _lookupRepo.GetCmvInvolvedLookups();
            return new ObjectResult(results);
        }

        [HttpGet("bikeInvolved")]
        public IActionResult GetBikeInvolvedLookups()
        {
            var results = _lookupRepo.GetBikeInvolvedLookups();
            return new ObjectResult(results);
        }
        [HttpGet("pedInvolved")]
        public IActionResult GetPedInvolvedLookups()
        {
            var results = _lookupRepo.GetPedInvolvedLookups();
            return new ObjectResult(results);
        }

        [HttpGet("crashSeverity")]
        public IActionResult GetCrashSeverityLookups()
        {
            var results = _lookupRepo.GetCrashSeverityLookups();
            return new ObjectResult(results);
        }

        [HttpGet("crashType")]
        public IActionResult GetCrashTypeLookups()
        {
            var results = _lookupRepo.GetCrashTypeLookups();
            return new ObjectResult(results);
        }

        [HttpGet("roadSysId")]
        public IActionResult GetRoadSysIdLookups()
        {
            var results = _lookupRepo.GetRoadSysIdLookups();
            return new ObjectResult(results);
        }

        [HttpGet("intersectionRelated")]
        public IActionResult GetIntersectionRelatedLookups()
        {
            var results = _lookupRepo.GetIntersectionRelatedLookups();
            return new ObjectResult(results);
        }

        [HttpGet("dayOrNight")]
        public IActionResult GetDayOrNightLookups()
        {
            var results = _lookupRepo.GetDayOrNightLookups();
            return new ObjectResult(results);
        }

        [HttpGet("behavioralFactors")]
        public IActionResult GetBehavioralFactorsLookups()
        {
            var results = _lookupRepo.GetBehavioralFactorsLookups();
            return new ObjectResult(results);
        }

        [HttpGet("laneDeparture")]
        public IActionResult GetLaneDepartureLookups()
        {
            var results = _lookupRepo.GetLaneDepartureLookups();
            return new ObjectResult(results);
        }

        [HttpGet("weatherCondition")]
        public IActionResult GetWeatherConditionLookups()
        {
            var results = _lookupRepo.GetWeatherConditionLookups();
            return new ObjectResult(results);
        }

        [HttpGet("dotDistrict")]
        public IActionResult GetDotDistrictLookups()
        {
            var results = _lookupRepo.GetDotDistrictLookups();
            return new ObjectResult(results);
        }

        [HttpGet("driverAgeRange")]
        public IActionResult GetDriverAgeRangeLookups()
        {
            var results = _lookupRepo.GetDriverAgeRangeLookups();
            return new ObjectResult(results);
        }

    }
}
