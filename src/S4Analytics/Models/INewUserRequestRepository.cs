using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace S4Analytics.Models
{
    public interface INewUserRequestRepository
    {
        IEnumerable<NewUserRequest> GetAll();
        IEnumerable<NewUserRequest> FilterBy(NewUserRequestStatus status);
        NewUserRequest Find(int reqNbr);
        int Update(int reqNbr, Dictionary<string, object> body);
    }
}
