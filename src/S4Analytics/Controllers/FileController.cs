using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using S4Analytics.Models;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace S4Analytics.Controllers
{
    [Authorize]
    public class FileController : Controller
    {
        ServerOptions _serverOptions;

        public FileController(IOptions<ServerOptions> serverOptions)
        {
            _serverOptions = serverOptions.Value;
        }

        /// <summary>
        /// Return file stream for the requested contract
        /// </summary>
        /// <param name="fileName">contract name</param>
        /// <returns></returns>
        [HttpGet("admin/new-user-request/contract-pdf/{fileName}")]
        [Authorize(Policy = "any admin")]
        public IActionResult GetContractPdf(string fileName)
        {
            var contractShareUrl = _serverOptions.ContractShare.Url;
            var username = _serverOptions.ContractShare.UserName;
            var password = _serverOptions.ContractShare.Password;

            var url = contractShareUrl + fileName;

            try
            {
                using (var client = new WebClient())
                {
                    client.Credentials = new NetworkCredential(username, password);

                    var data = client.DownloadData(url);
                    Stream stream = new MemoryStream(data);

                    var file = File(stream, "application/pdf");
                    return new ObjectResult(file.FileStream);
                }

            }
            catch(Exception ex)
            {
               return new ObjectResult(new MemoryStream(Encoding.UTF8.GetBytes($@"<div>Contract '{fileName}' not found</div>")));
            }

        }
    }
}
