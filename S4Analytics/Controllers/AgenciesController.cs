using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using S4Analytics.Models;

namespace S4Analytics.Controllers
{
    [Route("api/[controller]")]
    public class AgenciesController : Controller
    {
        public AgenciesController(IAgencyRepository agencies)
        {
            Agencies = agencies;
        }

        public IAgencyRepository Agencies { get; set; }

        [HttpGet]
        public IEnumerable<Agency> GetAll()
        {
            return Agencies.GetAll();
        }
    }
}
