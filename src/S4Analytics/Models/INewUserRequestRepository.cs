using S4Analytics.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace S4Analytics.Models
{

    public interface INewUserRequestRepository
    {
        Task<IEnumerable<NewUserRequest>> GetAll(string adminUserName);
        NewUserRequest Find(int reqNbr);
        Task<NewUserRequest> ApproveNewUser(int id, RequestApproval approval);
        Task<NewUserRequest> ApproveNewConsultant(int id, RequestApproval approval);
        NewUserRequest ApproveAgency(int id, RequestApproval approval);
        NewUserRequest ApproveNewVendor(int id, RequestApproval approval);
        Task<NewUserRequest> ApproveCreatedNewAgency(int id, RequestApproval approval);
        NewUserRequest Reject(int id, RequestRejection approval);
        int FindAgencyIdByName(string agencyNm);
    }
}
