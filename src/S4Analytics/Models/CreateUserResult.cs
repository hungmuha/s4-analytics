using System.Web.Security;

namespace S4Analytics.Models
{
    public class CreateUserResult
    {
        public MembershipCreateStatus Status { get; set; }
        public S4User S4User { get; set; }
    }
}
