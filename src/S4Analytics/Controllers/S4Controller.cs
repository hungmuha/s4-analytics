using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace S4Analytics.Controllers
{
    public abstract class S4Controller : Controller
    {
        /// <summary>
        /// Wrap the data in an anonymous-typed object with a single "Data" key.
        /// See https://www.owasp.org/index.php/OWASP_AJAX_Security_Guidelines#Always_return_JSON_with_an_Object_on_the_outside.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected object AjaxSafeData(object data)
        {
            return new { data };
        }
    }
}
