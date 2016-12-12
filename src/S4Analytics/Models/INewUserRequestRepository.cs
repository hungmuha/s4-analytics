
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace S4Analytics.Models
{
    public interface INewUserRequestRepository
    {
        IEnumerable<NewUserRequest> GetAll();
        NewUserRequest GetNewUserRequestById(string id);
        void UpdateRequestQueue(NewUserRequest newUserReq);
    }
}
