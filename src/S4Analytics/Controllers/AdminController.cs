using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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


        [HttpGet("new-user-request")]
        public IActionResult GetAllNewUserRequests()
        {
            var info = _newUserRequestRepo.GetAll();

            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }

        [HttpGet("new-user-request/{id}")]
        public IActionResult GetNewUserRequestById(string id)
        {
            var info = _newUserRequestRepo.GetNewUserRequestById(id);
            if (info == null)
            {
                return NotFound();
            }
            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }

        [HttpGet("new-user-request-nbr/{reqNbr}")]
        public IActionResult GetNewUserRequestByReqNbr(int reqNbr)
        {
            var info = _newUserRequestRepo.GetNewUserRequestByReqNbr(reqNbr);
            if (info == null)
            {
                return NotFound();
            }
            var data = AjaxSafeData(info);
            return new ObjectResult(data);
        }


    }
}
