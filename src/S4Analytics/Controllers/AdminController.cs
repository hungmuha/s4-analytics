using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System.Collections.Generic;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    public class AdminController : S4Controller
    {
        private INewUserRequestRepository _newUserRequestRepo;

        public AdminController(INewUserRequestRepository repo)
        {
            _newUserRequestRepo = repo;
        }

        /// <summary>
        /// Return all records from NEW_USER_REQ
        /// </summary>
        /// <returns></returns>
        [HttpGet("new-user-request")]
        public IActionResult GetAllNewUserRequests()
        {
            var info = _newUserRequestRepo.GetAll();

            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }

        /// <summary>
        /// Return record from NEW_USER_REQ where REQ_NBR = reqNbr
        /// </summary>
        /// <param name="reqNbr">request number of record to return</param>
        /// <returns></returns>
        [HttpGet("new-user-request/{reqNbr}")]
        public IActionResult GetNewUserRequestByReqNbr(int reqNbr)
        {
            var info = _newUserRequestRepo.Find(reqNbr);
            if (info == null)
            {
                return NotFound();
            }
            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }

        /// <summary>
        /// Return record from NEW_USER_REQ where REQ_NBR = reqNbr
        /// </summary>
        /// <param name="reqNbr">request number of record to return</param>
        /// <returns></returns>
        [HttpGet("new-user-request/filter/{status}", Name = "GetNewUserRequestByStatus")]
        public IActionResult GetNewUserRequestByStatus(NewUserRequestStatus status)
        {
            var info = _newUserRequestRepo.FilterBy(status);
            if (info == null)
            {
                return NotFound();
            }
            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }

        /// <summary>
        /// Update record in NEW_USER_REQ table
        /// </summary>
        /// <param name="reqNbr">record to update</param>
        /// <param name="body">fields to update</param>
        /// <returns></returns>
        [HttpPatch("new-user-request/{reqNbr}")]
        public IActionResult UpdateNewUserRequest(int reqNbr, [FromBody] Dictionary<string, object> body)
        {
            var result =  _newUserRequestRepo.Update(reqNbr,  body);

            if (result == 0)
            {
                return NotFound();
            }

            return new NoContentResult();
        }

    }
}
