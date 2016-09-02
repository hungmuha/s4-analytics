using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using Lib.PBCAT;

namespace S4Analytics.Controllers
{
    public class PedestrianInfoWrapper
    {
        public int HsmvReportNumber { get; set; }
        public PBCATPedestrianInfo PedestrianInfo { get; set; }
        public CrashTypePedestrian PedestrianCrashType { get; set; }
    }

    [Route("api/[controller]")]
    public class PbcatController : Controller
    {
        public PbcatController(IPbcatPedRepository pedRepo)
        {
            PedRepo = pedRepo;
        }

        public IPbcatPedRepository PedRepo { get; set; }

        /// <summary>
        /// GET /api/pbcat/ped/:hsmvRptNbr
        /// Return an existing PBCAT_PED record from the database given its HSMV report number.
        /// </summary>
        [HttpGet("ped/{hsmvRptNbr}", Name = "GetPedestrianInfo")]
        public IActionResult GetPedestrianInfo(int hsmvRptNbr)
        {
            var pedInfo = PedRepo.Find(hsmvRptNbr);
            if (pedInfo == null)
            {
                return NotFound();
            }

            return new ObjectResult(pedInfo);
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
            var crashReportExists = PedRepo.CrashReportExists(hsmvRptNbr);
            if (!crashReportExists)
            {
                return NotFound();
            }

            // ped info must not exist
            var existingPedInfo = PedRepo.Find(hsmvRptNbr);
            if (existingPedInfo != null)
            {
                return StatusCode((int)HttpStatusCode.Conflict);
            }

            PedRepo.Add(hsmvRptNbr, pedInfo, crashType);
            return CreatedAtRoute("GetPedestrianInfo", new { hsmvRptNbr }, pedInfoWrapper);
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

            var existingPedInfo = PedRepo.Find(hsmvRptNbr);
            if (existingPedInfo == null)
            {
                return NotFound();
            }

            PedRepo.Update(hsmvRptNbr, pedInfo, crashType);
            return new NoContentResult();
        }

        /// <summary>
        /// DELETE /api/pbcat/ped/:hsmvRptNbr
        /// Delete an existing PBCAT_PED record from the database given its HSMV report number.
        /// </summary>
        [HttpDelete("ped/{hsmvRptNbr}")]
        public IActionResult DeletePedestrianInfo(int hsmvRptNbr)
        {
            var pedInfo = PedRepo.Find(hsmvRptNbr);
            if (pedInfo == null)
            {
                return NotFound();
            }

            PedRepo.Remove(hsmvRptNbr);
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

            var crashType = PedRepo.GetCrashType(pedInfo);
            return CreatedAtRoute("CalculatePedestrianCrashType", crashType);
        }
    }
}
