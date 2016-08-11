using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    public class VersionController : Controller
    {
        private string _appVersion;

        public VersionController(IOptions<AppOptions> appOptions)
        {
            _appVersion = appOptions.Value.Version;
        }

        [HttpGet]
        public string GetVersion()
        {
            return _appVersion;
        }
    }
}
