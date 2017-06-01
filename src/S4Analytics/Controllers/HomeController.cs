using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Lib.Identity;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Text;

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
