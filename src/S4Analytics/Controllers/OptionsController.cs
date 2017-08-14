using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class OptionsController : Controller
    {
        private IOptions<ClientOptions> _clientOptions;

        public OptionsController(IOptions<ClientOptions> clientOptions)
        {
            _clientOptions = clientOptions;
        }

        [HttpGet]
        public IActionResult GetOptions()
        {
            return new ObjectResult(_clientOptions.Value);
        }
    }
}
