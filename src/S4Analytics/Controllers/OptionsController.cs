using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
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

        [HttpGet("date")]
        public IActionResult GetCurrentDate()
        {
            var response = new { date = DateTime.Now.Date };
            return new ObjectResult(response);
        }

        [HttpGet("time")]
        public IActionResult GetCurrentTime()
        {
            var response = new { time = DateTime.Now };
            return new ObjectResult(response);
        }
    }
}
