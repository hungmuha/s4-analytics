using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Lib.Identity;
using Microsoft.AspNetCore.Identity;

namespace S4Analytics.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        SignInManager<S4IdentityUser> _signInManager;

        public HomeController(SignInManager<S4IdentityUser> signInManager, IHostingEnvironment env)
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
        /// Log in. Local environment only, as no password is required.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpGet("api/login/{userName}")]
        public async Task<IActionResult> LogIn(string userName)
        {
            if (_env.EnvironmentName == "Local")
            {
                await _signInManager.SignInAsync(new S4IdentityUser(userName), isPersistent: false);
            }
            return new NoContentResult();
        }

        /// <summary>
        /// Log out currently authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/logout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return new NoContentResult();
        }
    }
}
