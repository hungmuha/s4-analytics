using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using AspNetCore.Identity.Oracle;
using Microsoft.AspNetCore.Identity;

namespace S4Analytics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        SignInManager<OracleIdentityUser> _signInManager;

        public HomeController(SignInManager<OracleIdentityUser> signInManager, IHostingEnvironment env)
        {
            _signInManager = signInManager;
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Login for local testing only. No password is required.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("login/{userName}")]
        public async Task<IActionResult> Login(string userName)
        {
            if (_env.EnvironmentName == "Local")
            {
                await _signInManager.SignInAsync(new OracleIdentityUser(userName), isPersistent: false);
            }
            return new NoContentResult();
        }
    }
}
