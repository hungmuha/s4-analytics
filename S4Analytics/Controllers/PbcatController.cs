using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;
using Lib.PBCAT;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    public class PbcatController : Controller
    {
        public PbcatController(IPbcatRepository pbcatItems)
        {
            // constructor
        }
    }
}
