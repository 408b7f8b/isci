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
    public class Applications
    {
        public static System.Net.Http.HttpClient client;
        public static string target_path;

        static async System.Threading.Tasks.Task GetToFile(string pfad, string ziel)
        {
        // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                var response = await client.GetAsync(pfad);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsByteArrayAsync();
                System.IO.File.WriteAllBytes(ziel, responseBody);
                Console.WriteLine(responseBody);
            }
            catch(System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        public Applications()
        {
            target_path = "applications";
        }

        [HttpPost("anforderung")]
        public void Anforderung([FromBody]Dictionary<string, string> request)
        {
            try
            {
                //var req = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
                var befehl = request["Cmd"];
                switch (befehl)
                {
                    case "anlegen":
                    var task = GetToFile(request["Url"], request["Identifikation"]);
                    //task.Start();
                    task.Wait();
                    break;
                    case "loeschen": Loeschen(request["Identifikation"]); break;
                }
            } catch {

            }
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            var list = General.FoldersInPath(target_path);

            return list;
        }

        [HttpGet("{Identifikation}/{*lol}")]
        public ActionResult<string> Get(string Identifikation, string lol = "")
        {
            if (Identifikation.Contains("//")) return "";

            var pfad = target_path;
            if (lol != "") pfad += "/" + lol;

            switch (Identifikation)
            {
                case "Ordner":
                case "ordner": return General.FoldersInPath(pfad);
                case "Dateien":
                case "dateien": return General.FilesInPath(pfad);
                default: break;
            }

            string ret = "";
            try 
            {
                ret = System.IO.File.ReadAllText(pfad);
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
        
        /*[HttpGet]
        new public ActionResult<string> Get()
        {
            var list = General.FoldersInPath(target_path);

            return list;
        }*/
    }
}
