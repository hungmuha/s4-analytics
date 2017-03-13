using System.Collections.Generic;

namespace S4Analytics.Models
{

    public interface INewUserRequestRepository
    {
        IEnumerable<NewUserRequest> GetAll();
        NewUserRequest Find(int reqNbr);
        NewUserRequest ApproveNewUser(int id, NewUserRequestStatus newStatus, NewUserRequest selectedRequest);
        NewUserRequest ApproveNewConsultant(int id, bool before70days, NewUserRequestStatus newStatus, NewUserRequest selectedRequest);
        NewUserRequest ApproveAgency(int id, bool before70days, bool lea, NewUserRequestStatus newStatus, NewUserRequest selectedRequest);
        NewUserRequest ApproveNewContractor(int id, NewUserRequestStatus newStatus, NewUserRequest selectedRequest);
        NewUserRequest ApproveCreatedNewAgency(int id, NewUserRequestStatus newStatus, NewUserRequest selectedRequest);
    }
}
