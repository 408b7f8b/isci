using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Dateien
    {
        public string target_path;

        public Dateien() : base()
        {
            target_path = "Dateien";
        }

        [HttpGet("{*Identifikation}")]
        public FileContentResult Get(string Identifikation)
        {
            var ret = new FileContentResult(new byte[1]{0}, "application/octet-stream");

            if (Identifikation.Contains("//")) return ret;

            try 
            {
                ret = new FileContentResult(System.IO.File.ReadAllBytes(Identifikation), "application/octet-stream");
            }
            catch {

            }

            return ret;
        }
    }
}
