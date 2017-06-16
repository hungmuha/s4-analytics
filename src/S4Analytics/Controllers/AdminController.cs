using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System.Threading.Tasks;

namespace S4Analytics.Controllers
{
    public enum QueueFilter
    {
        All,
        Pending,
        Completed,
        Rejected
    }

    public class RequestApproval
    {
        public int RequestNumber { get; set; }
        public NewUserRequest SelectedRequest { get; set; }
        public NewUserRequestStatus NewStatus { get; set; }
        public NewUserRequestStatus CurrentStatus { get; set; }
        public bool Before70Days { get; set; }
        public bool Lea { get; set; }
    }

    public class RequestRejection
    {
        public int RequestNumber{ get; set;}
        public NewUserRequest SelectedRequest{ get; set;}
        public string RejectionReason { get; set; }
        public NewUserRequestStatus NewStatus { get; set; }
    }

    [Route("api/[controller]")]
    [Authorize(Policy = "any admin")]
    public class AdminController : Controller
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
            return new ObjectResult(info);
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
            return new ObjectResult(info);
        }

        [HttpPatch("new-user-request/{id}/approve")]
        public async Task<IActionResult> ApproveOther(int id, [FromBody]RequestApproval approval)
        {
            var currentStatus = approval.CurrentStatus;
            var newStatus = approval.NewStatus;
            var selectedRequest = approval.SelectedRequest;

            switch(currentStatus)
            {
                case NewUserRequestStatus.NewUser:
                    return new ObjectResult(await _newUserRequestRepo.ApproveNewUser(id, approval));
                case NewUserRequestStatus.NewContractor:
                    return new ObjectResult(_newUserRequestRepo.ApproveNewContractor(id, approval));
                case NewUserRequestStatus.CreateAgency:
                    return new ObjectResult(await _newUserRequestRepo.ApproveCreatedNewAgency(id, approval));
            }

            return null;
        }

        [HttpPatch("new-user-request/{id}/approve/consultant")]
        public async Task<IActionResult> ApproveConsultant(int id, [FromBody]RequestApproval approval)
        {
            return new ObjectResult(await _newUserRequestRepo.ApproveNewConsultant(id, approval));
        }

        [HttpPatch("new-user-request/{id}/approve/agency")]
        public IActionResult ApproveAgency(int id, [FromBody]RequestApproval approval)
        {
            return new ObjectResult(_newUserRequestRepo.ApproveAgency(id, approval));
        }

        [HttpPatch("new-user-request/{id}/reject")]
        public IActionResult Reject(int id, [FromBody]RequestRejection rejection)
        {
            return new ObjectResult(_newUserRequestRepo.Reject(id,  rejection));
        }
    }
}
