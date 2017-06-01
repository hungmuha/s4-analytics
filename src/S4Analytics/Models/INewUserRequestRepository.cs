﻿using S4Analytics.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S4Analytics.Models
{

    public interface INewUserRequestRepository
    {
        IEnumerable<NewUserRequest> GetAll();
        NewUserRequest Find(int reqNbr);
        NewUserRequest ApproveNewUser(int id, RequestApproval approval);
        Task<NewUserRequest> ApproveNewConsultant(int id, RequestApproval approval);
        NewUserRequest ApproveAgency(int id, RequestApproval approval);
        NewUserRequest ApproveNewContractor(int id, RequestApproval approval);
        NewUserRequest ApproveCreatedNewAgency(int id, RequestApproval approval);
        NewUserRequest Reject(int id, RequestRejection approval);
    }
}
