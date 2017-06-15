using Lib.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace S4Analytics.Controllers
{
    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHostingEnvironment _env;
        SignInManager<S4IdentityUser<S4UserProfile>> _signInManager;
        UserManager<S4IdentityUser<S4UserProfile>> _userManager;

        public HomeController(
            SignInManager<S4IdentityUser<S4UserProfile>> signInManager,
            UserManager<S4IdentityUser<S4UserProfile>> userManager,
            IHostingEnvironment env)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _env = env;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Log in.
        /// </summary>
        /// <param name="credentials">Username and password</param>
        /// <returns></returns>
        [HttpPost("api/login")]
        [AllowAnonymous]
        public async Task<IActionResult> LogIn([FromBody] Credentials credentials)
        {
            bool success = false;
            var user = await _userManager.FindByNameAsync(credentials.UserName);
            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, credentials.Password, isPersistent: false, lockoutOnFailure: false);
                success = signInResult == Microsoft.AspNetCore.Identity.SignInResult.Success;
            }
            if (!success)
            {
                await _signInManager.SignOutAsync();
            }
            return new ObjectResult(new { success });
        }

        /// <summary>
        /// Log out currently authenticated user.
        /// </summary>
        /// <returns></returns>
        [HttpPost("api/logout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return new NoContentResult();
        }

        [HttpGet("api/current-user")]
        public IActionResult GetCurrentUser()
        {
            var userName = _userManager.GetUserName(User);
            return new ObjectResult(new { userName });
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
