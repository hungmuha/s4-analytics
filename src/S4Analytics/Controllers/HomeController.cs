using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Lib.Identity.Models;
using Microsoft.AspNetCore.Identity;
using S4Analytics.Models;
using System.IO;
using System.Text;

namespace S4Analytics.Controllers
{
    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        SignInManager<S4IdentityUser<S4UserProfile>> _signInManager;
        UserManager<S4IdentityUser<S4UserProfile>> _userManager;
        IUserStore<S4IdentityUser<S4UserProfile>> _userStore;

        public HomeController(
            SignInManager<S4IdentityUser<S4UserProfile>> signInManager,
            UserManager<S4IdentityUser<S4UserProfile>> userManager,
            IUserStore<S4IdentityUser<S4UserProfile>> userStore,
            IHostingEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
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
        [HttpPost("api/login")]
        public async Task<IActionResult> LogIn([FromBody] Credentials credentials)
        {
            var user = await _userManager.FindByNameAsync(credentials.UserName);
            var signInResult = await _signInManager.PasswordSignInAsync(user, credentials.Password, false, false);
            var success = signInResult == Microsoft.AspNetCore.Identity.SignInResult.Success;
            return new ObjectResult(new { success });
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

        /// <summary>
        /// Return file stream for the requested contract
        /// </summary>
        /// <param name="fileName">contract name</param>
        /// <returns></returns>
        [HttpGet("admin/new-user-request/contract-pdf/{fileName}")]
        public IActionResult GetContractPdf(string fileName)
        {
            // TODO: get correct path here
            var path = $@"D:\Git\S4-Analytics\S4.Analytics.Web\Uploads\{fileName}";

            if (!System.IO.File.Exists(path))
            {
                return new ObjectResult(new MemoryStream(Encoding.UTF8.GetBytes($@"<div>{fileName} not found</div>")));
            }

            var stream = System.IO.File.Open(path, FileMode.Open);

            var file = File(stream, "application/pdf");
            return new ObjectResult(file.FileStream);
        }
    }
}
