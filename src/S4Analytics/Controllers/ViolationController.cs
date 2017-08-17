using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ViolationController : Controller
    {

    }
}
