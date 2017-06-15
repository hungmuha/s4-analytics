using Lib.Identity.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using System.Threading.Tasks;

namespace S4Analytics.Controllers
{
    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    [Route("api/[controller]")]
    [Authorize]
    public class IdentityController : Controller
    {
        SignInManager<S4IdentityUser<S4UserProfile>> _signInManager;
        UserManager<S4IdentityUser<S4UserProfile>> _userManager;

        public IdentityController(
            SignInManager<S4IdentityUser<S4UserProfile>> signInManager,
            UserManager<S4IdentityUser<S4UserProfile>> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        /// <summary>
        /// Log in.
        /// </summary>
        /// <param name="credentials">Username and password</param>
        /// <returns></returns>
        [HttpPost("login")]
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
        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return new ObjectResult(new { success = true });
        }

        [HttpGet("current-user")]
        public IActionResult GetCurrentUser()
        {
            var userName = _userManager.GetUserName(User);
            return new ObjectResult(new { userName });
        }
    }
}
