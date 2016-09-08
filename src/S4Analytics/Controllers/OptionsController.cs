using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    public class OptionsController : S4Controller
    {
        private IOptions<ClientOptions> _clientOptions;

        public OptionsController(IOptions<ClientOptions> clientOptions)
        {
            _clientOptions = clientOptions;
        }

        [HttpGet]
        public IActionResult GetOptions()
        {
            var data = AjaxSafeData(_clientOptions.Value);
            return new ObjectResult(data);
        }
    }
}
