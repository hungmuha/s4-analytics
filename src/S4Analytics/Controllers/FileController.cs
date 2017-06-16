using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;

namespace S4Analytics.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        /// <summary>
        /// Return file stream for the requested contract
        /// </summary>
        /// <param name="fileName">contract name</param>
        /// <returns></returns>
        [HttpGet("admin/new-user-request/contract-pdf/{fileName}")]
        [Authorize(Policy = "any admin")]
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
