using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

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
    }

    public class NewAgencyRequestApproval : RequestApproval
    {
        public bool Before70Days { get; set;}
        public bool Lea { get; set; }
    }

    public class NewConsultantRequestApproval : RequestApproval
    {
        public bool Before70Days { get; set; }
    }

    [Route("api/[controller]")]
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
        public IActionResult ApproveOther(int id, [FromBody]RequestApproval approval)
        {
            var currentStatus = approval.CurrentStatus;
            var newStatus = approval.NewStatus;
            var selectedRequest = approval.SelectedRequest;

            switch(currentStatus)
            {
                case NewUserRequestStatus.NewUser:
                    _newUserRequestRepo.ApproveNewUser(id, newStatus, selectedRequest);
                    break;
                case NewUserRequestStatus.NewContractor:
                    _newUserRequestRepo.ApproveNewContractor(id, newStatus, selectedRequest);
                    break;
                case NewUserRequestStatus.CreateAgency:
                    _newUserRequestRepo.ApproveCreatedNewAgency(id, newStatus, selectedRequest);
                    break;
            }

            return null;
        }

        [HttpPatch("new-user-request/{id}/approve/consultant")]
        public IActionResult ApproveConsultant(int id, [FromBody]NewConsultantRequestApproval approval)
        {
            _newUserRequestRepo.ApproveNewConsultant(id, approval.Before70Days, approval.NewStatus, approval.SelectedRequest);

            return null;
        }

        [HttpPatch("new-user-request/{id}/approve/agency")]
        public IActionResult ApproveAgency(int id, [FromBody]NewAgencyRequestApproval approval)
        {
            _newUserRequestRepo.ApproveAgency(id, approval.Before70Days, approval.Lea, approval.NewStatus, approval.SelectedRequest);

            return null;
        }

    }

}
