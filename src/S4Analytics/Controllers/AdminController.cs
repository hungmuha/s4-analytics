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

        [HttpPatch("new-user-request/{id}/approve")]
        public IActionResult ApproveOther(int id, [FromBody]RequestApproval wrapper)
        {
            var currentStatus = wrapper.CurrentStatus;
            var newStatus = wrapper.NewStatus;

            switch(currentStatus)
            {
                case NewUserRequestStatus.NewUser:
                    _newUserRequestRepo.ApproveNewUser(id, newStatus);
                    break;
                case NewUserRequestStatus.NewContractor:
                    _newUserRequestRepo.ApproveNewContractor(id, newStatus);
                    break;
                case NewUserRequestStatus.CreateAgency:
                    _newUserRequestRepo.ApproveCreateNewAgency(id, newStatus);
                    break;
            }

            return null;
        }

        [HttpPatch("new-user-request/{id}/approve/consultant")]
        public IActionResult ApproveConsultant(int id, [FromBody]NewConsultantRequestApproval wrapper)
        {
            _newUserRequestRepo.ApproveNewConsultant(id, wrapper.Before70Days, wrapper.NewStatus);

            return null;
        }

        [HttpPatch("new-user-request/{id}/approve/agency")]
        public IActionResult ApproveAgency(int id, [FromBody]NewAgencyRequestApproval wrapper)
        {
            _newUserRequestRepo.ApproveNewAgency(id, wrapper.Before70Days, wrapper.Lea, wrapper.NewStatus);

            return null;
        }

    }

}
