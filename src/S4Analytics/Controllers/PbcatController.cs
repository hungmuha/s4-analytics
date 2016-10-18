using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using Lib.PBCAT;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Oracle;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Claims;

namespace S4Analytics.Controllers
{
    public class PedestrianInfoWrapper
    {
        public int HsmvReportNumber { get; set; }
        public PBCATPedestrianInfo PedestrianInfo { get; set; }
        public CrashTypePedestrian PedestrianCrashType { get; set; }
    }

    public class BicyclistInfoWrapper
    {
        public int HsmvReportNumber { get; set; }
        public PBCATBicyclistInfo BicyclistInfo { get; set; }
        public CrashTypeBicyclist BicyclistCrashType { get; set; }
    }

    [Authorize]
    [Route("api/[controller]")]
    public class PbcatController : S4Controller
    {
        // COMMON
        private readonly UserManager<OracleIdentityUser> _userManager;
        private readonly SignInManager<OracleIdentityUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PbcatController(
            UserManager<OracleIdentityUser> userManager,
            SignInManager<OracleIdentityUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            IPbcatRepository pbcatRepo)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            PbcatRepo = pbcatRepo;
        }

        public IPbcatRepository PbcatRepo { get; set; }

        [AllowAnonymous]
        [HttpGet("session/{token}")]
        public async Task<IActionResult> GetSession(string token)
        {
            var tokenAsGuid = Guid.Parse(token);
            var session = PbcatRepo.GetSession(tokenAsGuid);
            if (session.UserName.Length > 0)
            {
                await _signInManager.SignInAsync(new OracleIdentityUser(session.UserName), isPersistent: false);
            }
            return Content(session.QueueJson);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("{hsmvRptNbr}")]
        public IActionResult GetParticipantInfo(int hsmvRptNbr)
        {
            var info = PbcatRepo.GetParticipantInfo(hsmvRptNbr);
            if (info == null)
            {
                return NotFound();
            }
            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }

        // PEDESTRIAN

        /// <summary>
        /// GET /api/pbcat/ped/:hsmvRptNbr
        /// Return an existing PBCAT_PED record from the database given its HSMV report number.
        /// </summary>
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("ped/{hsmvRptNbr}", Name = "GetPedestrianInfo")]
        public IActionResult GetPedestrianInfo(int hsmvRptNbr)
        {
            var pedInfo = PbcatRepo.FindPedestrian(hsmvRptNbr);
            if (pedInfo == null)
            {
                return NotFound();
            }

            var data = AjaxSafeData(pedInfo);
            return new ObjectResult(data);
        }

        /// <summary>
        /// POST /api/pbcat/ped
        /// Insert a new PBCAT_PED record into the database.
        /// </summary>
        [HttpPost("ped")]
        public IActionResult CreatePedestrianInfo([FromBody] PedestrianInfoWrapper pedInfoWrapper)
        {
            if (pedInfoWrapper == null)
            {
                return BadRequest();
            }

            var hsmvRptNbr = pedInfoWrapper.HsmvReportNumber;
            var pedInfo = pedInfoWrapper.PedestrianInfo;
            var crashType = pedInfoWrapper.PedestrianCrashType;

            // crash report must exist
            var crashReportExists = PbcatRepo.CrashReportExists(hsmvRptNbr);
            if (!crashReportExists)
            {
                return NotFound();
            }

            // ped info must not exist
            var existingPedInfo = PbcatRepo.FindPedestrian(hsmvRptNbr);
            if (existingPedInfo != null)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            PbcatRepo.Add(hsmvRptNbr, userName, pedInfo, crashType);
            var data = AjaxSafeData(pedInfoWrapper);
            return CreatedAtRoute("GetPedestrianInfo", new { hsmvRptNbr }, data);
        }

        /// <summary>
        /// PUT /api/pbcat/ped/:hsmvRptNbr
        /// Update an existing PBCAT_PED record in the database.
        /// </summary>
        [HttpPut("ped/{hsmvRptNbr}")]
        public IActionResult UpdatePedestrianInfo(int hsmvRptNbr, [FromBody] PedestrianInfoWrapper pedInfoWrapper)
        {
            if (pedInfoWrapper == null)
            {
                return BadRequest();
            }

            var pedInfo = pedInfoWrapper.PedestrianInfo;
            var crashType = pedInfoWrapper.PedestrianCrashType;

            var existingPedInfo = PbcatRepo.FindPedestrian(hsmvRptNbr);
            if (existingPedInfo == null)
            {
                return NotFound();
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            PbcatRepo.Update(hsmvRptNbr, userName, pedInfo, crashType);
            return new NoContentResult();
        }

        /// <summary>
        /// DELETE /api/pbcat/ped/:hsmvRptNbr
        /// Delete an existing PBCAT_PED record from the database given its HSMV report number.
        /// </summary>
        [HttpDelete("ped/{hsmvRptNbr}")]
        public IActionResult DeletePedestrianInfo(int hsmvRptNbr)
        {
            var pedInfo = PbcatRepo.FindPedestrian(hsmvRptNbr);
            if (pedInfo == null)
            {
                return NotFound();
            }

            PbcatRepo.RemovePedestrian(hsmvRptNbr);
            return new NoContentResult();
        }

        /// <summary>
        /// POST /api/pbcat/ped/crashtype
        /// Calculate the CRASH_TYPE_NBR, CRASH_GROUP_NBR, CRASH_TYPE_EXPANDED and
        /// CRASH_GROUP_EXPANDED for the PBCAT_PED record, but do not update the database.
        /// </summary>
        [HttpPost("ped/crashtype", Name = "CalculatePedestrianCrashType")]
        public IActionResult CalculatePedestrianCrashType([FromBody] PBCATPedestrianInfo pedInfo)
        {
            if (pedInfo == null)
            {
                return BadRequest();
            }

            var crashType = PbcatRepo.GetCrashType(pedInfo);
            var data = AjaxSafeData(crashType);
            return CreatedAtRoute("CalculatePedestrianCrashType", data);
        }

        // BICYCLIST

        /// <summary>
        /// GET /api/pbcat/bike/:hsmvRptNbr
        /// Return an existing PBCAT_BIKE record from the database given its HSMV report number.
        /// </summary>
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("bike/{hsmvRptNbr}", Name = "GetBicyclistInfo")]
        public IActionResult GetBicyclistInfo(int hsmvRptNbr)
        {
            var bikeInfo = PbcatRepo.FindBicyclist(hsmvRptNbr);
            if (bikeInfo == null)
            {
                return NotFound();
            }

            var data = AjaxSafeData(bikeInfo);
            return new ObjectResult(data);
        }

        /// <summary>
        /// POST /api/pbcat/bike
        /// Insert a new PBCAT_BIKE record into the database.
        /// </summary>
        [HttpPost("bike")]
        public IActionResult CreateBicyclistInfo([FromBody] BicyclistInfoWrapper bikeInfoWrapper)
        {
            if (bikeInfoWrapper == null)
            {
                return BadRequest();
            }

            var hsmvRptNbr = bikeInfoWrapper.HsmvReportNumber;
            var bikeInfo = bikeInfoWrapper.BicyclistInfo;
            var crashType = bikeInfoWrapper.BicyclistCrashType;

            // crash report must exist
            var crashReportExists = PbcatRepo.CrashReportExists(hsmvRptNbr);
            if (!crashReportExists)
            {
                return NotFound();
            }

            // bike info must not exist
            var existingBikeInfo = PbcatRepo.FindBicyclist(hsmvRptNbr);
            if (existingBikeInfo != null)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            PbcatRepo.Add(hsmvRptNbr, userName, bikeInfo, crashType);
            var data = AjaxSafeData(bikeInfoWrapper);
            return CreatedAtRoute("GetBicyclistInfo", new { hsmvRptNbr }, data);
        }

        /// <summary>
        /// PUT /api/pbcat/bike/:hsmvRptNbr
        /// Update an existing PBCAT_BIKE record in the database.
        /// </summary>
        [HttpPut("bike/{hsmvRptNbr}")]
        public IActionResult UpdateBicyclistInfo(int hsmvRptNbr, [FromBody] BicyclistInfoWrapper bikeInfoWrapper)
        {
            if (bikeInfoWrapper == null)
            {
                return BadRequest();
            }

            var bikeInfo = bikeInfoWrapper.BicyclistInfo;
            var crashType = bikeInfoWrapper.BicyclistCrashType;

            var existingbikeInfo = PbcatRepo.FindBicyclist(hsmvRptNbr);
            if (existingbikeInfo == null)
            {
                return NotFound();
            }

            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            PbcatRepo.Update(hsmvRptNbr, userName, bikeInfo, crashType);
            return new NoContentResult();
        }

        /// <summary>
        /// DELETE /api/pbcat/bike/:hsmvRptNbr
        /// Delete an existing PBCAT_BIKE record from the database given its HSMV report number.
        /// </summary>
        [HttpDelete("bike/{hsmvRptNbr}")]
        public IActionResult DeleteBicyclistInfo(int hsmvRptNbr)
        {
            var bikeInfo = PbcatRepo.FindBicyclist(hsmvRptNbr);
            if (bikeInfo == null)
            {
                return NotFound();
            }

            PbcatRepo.RemoveBicyclist(hsmvRptNbr);
            return new NoContentResult();
        }

        /// <summary>
        /// POST /api/pbcat/bike/crashtype
        /// Calculate the CRASH_TYPE_NBR, CRASH_GROUP_NBR, CRASH_TYPE_EXPANDED and
        /// CRASH_GROUP_EXPANDED for the PBCAT_BIKE record, but do not update the database.
        /// </summary>
        [HttpPost("bike/crashtype", Name = "CalculateBicyclistCrashType")]
        public IActionResult CalculateBicyclistCrashType([FromBody] PBCATBicyclistInfo bikeInfo)
        {
            if (bikeInfo == null)
            {
                return BadRequest();
            }

            var crashType = PbcatRepo.GetCrashType(bikeInfo);
            var data = AjaxSafeData(crashType);
            return CreatedAtRoute("CalculateBicyclistCrashType", data);
        }
    }
}
