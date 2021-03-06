﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System;
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
        public NewUserRequest Request { get; set; }
        public NewUserRequestStatus NewStatus { get; set; }
        public NewUserRequestStatus CurrentStatus { get; set; }
        public bool Before70Days { get; set; }
        public bool Lea { get; set; }
        public DateTime ContractEndDt { get; set; }
        public string AdminUserName { get; set; }
    }

    public class RequestRejection
    {
        public int RequestNumber{ get; set;}
        public NewUserRequest Request{ get; set;}
        public string RejectionReason { get; set; }
        public NewUserRequestStatus NewStatus { get; set; }
        public string AdminUserName { get; set; }
    }

    [Route("api/[controller]")]
    [Authorize(Policy = "any admin")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AdminController : Controller
    {
        private NewUserRequestRepository _newUserRequestRepo;

        public AdminController(NewUserRequestRepository repo)
        {
            _newUserRequestRepo = repo;
        }

        /// <summary>
        /// Return all records from NEW_USER_REQ
        /// </summary>
        /// <returns></returns>
        [HttpGet("new-user-request")]
        public async Task<IActionResult> GetAllNewUserRequests()
        {
            var info = await _newUserRequestRepo.GetAll(User.Identity.Name);
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

        [HttpGet("new-user-request/{agencyNm}/verify-agency")]
        public IActionResult  DoesAgencyExists(string agencyNm)
        {
            var agencyId = _newUserRequestRepo.FindAgencyIdByName(agencyNm);

            return new ObjectResult(agencyId != 0);
        }

        [HttpPatch("new-user-request/{id}/approve")]
        public async Task<IActionResult> ApproveOther(int id, [FromBody]RequestApproval approval)
        {
            var currentStatus = approval.CurrentStatus;
            approval.AdminUserName = User.Identity.Name;

            switch (currentStatus)
            {
                case NewUserRequestStatus.NewUser:
                    return new ObjectResult(await _newUserRequestRepo.ApproveNewUser(id, approval));
                case NewUserRequestStatus.CreateAgency:
                    return new ObjectResult(await _newUserRequestRepo.ApproveCreatedNewAgency(id, approval));
            }

            return null;
        }

        [HttpPatch("new-user-request/{id}/approve/vendor")]
        public async Task<IActionResult> ApproveVendor(int id, [FromBody]RequestApproval approval)
        {
            approval.AdminUserName = User.Identity.Name;
            return new ObjectResult(await _newUserRequestRepo.ApproveNewVendor(id, approval));
        }

        [HttpPatch("new-user-request/{id}/approve/consultant")]
        public async Task<IActionResult> ApproveConsultant(int id, [FromBody]RequestApproval approval)
        {
            approval.AdminUserName = User.Identity.Name;
            return new ObjectResult(await _newUserRequestRepo.ApproveConsultant(id, approval));
        }

        [HttpPatch("new-user-request/{id}/approve/agency")]
        public IActionResult ApproveAgency(int id, [FromBody]RequestApproval approval)
        {
            approval.AdminUserName = User.Identity.Name;
            return new ObjectResult(_newUserRequestRepo.ApproveAgency(id, approval));
        }

        [HttpPatch("new-user-request/{id}/reject")]
        public IActionResult Reject(int id, [FromBody]RequestRejection rejection)
        {
            rejection.AdminUserName = User.Identity.Name;
            return new ObjectResult(_newUserRequestRepo.Reject(id,  rejection));
        }
    }
}
