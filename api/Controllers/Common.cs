using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    public abstract class Common : ControllerBase
    {
        public static string target_path = "";

        [HttpGet]
        public ActionResult<string> Get()
        {
            var list = General.FilesInPath(target_path);

            return list;
        }

        [HttpGet("{Identifikation}")]
        public ActionResult<string> Get(string Identifikation)
        {
            if (Identifikation.Contains("//")) return "";

            string ret = "";
            try 
            {
                ret = System.IO.File.ReadAllText(target_path + "/" + Identifikation);
            }
            catch {

            }

            return ret;
        }

        [HttpPut("{Identifikation}")]
        public void Put(string Identifikation, [FromBody]string content)
        {
            if (Identifikation.Contains("//")) return;

            try 
            {
                System.IO.File.WriteAllText(target_path + "/" + Identifikation, content);
            }
            catch {

            }
        }

        [HttpPost("{Identifikation}")]
        public void Post(string Identifikation, [FromBody]string content)
        {
            if (Identifikation.Contains("//")) return;

            try 
            {
                System.IO.File.WriteAllText(target_path + "/" + Identifikation, content);
            }
            catch {

            }
        }

        [HttpDelete("{Identifikation}")]
        public void Loeschen(string Identifikation)
        {
            if (Identifikation.Contains("//")) return;

            try 
            {
                System.IO.File.Delete(target_path + "/" + Identifikation);
            }
            catch {

            }
        }
    }
}
