using System.Collections.Generic;

namespace S4Analytics.Models
{

    public interface INewUserRequestRepository
    {
        IEnumerable<NewUserRequest> GetAll();
        NewUserRequest Find(int reqNbr);
        NewUserRequest ApproveNewUser(int id, NewUserRequestStatus newStatus);
        NewUserRequest ApproveNewConsultant(int id, bool before70days, NewUserRequestStatus newStatus);
        NewUserRequest ApproveNewAgency(int id, bool before70days, bool lea, NewUserRequestStatus newStatus);
        NewUserRequest ApproveNewContractor(int id, NewUserRequestStatus newStatus);
        NewUserRequest ApproveCreateNewAgency(int id, NewUserRequestStatus newStatus);
    }
}
