
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace S4Analytics.Models
{
    public interface INewUserRequestRepository
    {
        IEnumerable<NewUserRequest> GetAll();
        NewUserRequest GetNewUserRequestById(string id);
        NewUserRequest GetNewUserRequestByReqNbr(int reqNbr);
        void UpdateRequestQueue(NewUserRequest newUserReq);
    }
}
