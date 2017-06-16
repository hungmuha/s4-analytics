using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ViolationController : Controller
    {

    }
}
