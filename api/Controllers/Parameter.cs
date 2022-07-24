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
    public class Parameter
    {
        public static string target_path;

        [HttpPost("anforderung")]
        public void Anforderung([FromBody]string request)
        {
            try
            {
                var req = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(request);
                var befehl = (string)req["Cmd"];
                var ordner = ((string)req["Bezugspfad"]).Replace("$APPLICATIONS", Applications.target_path);
                if (ordner.EndsWith("/")) ordner = ordner.Substring(0, ordner.Length-1);
                var rekursiv = ((string)req["Rekursiv"] == "true");
                var files = System.IO.Directory.GetFiles(ordner, "*.json", rekursiv ? System.IO.SearchOption.AllDirectories : System.IO.SearchOption.TopDirectoryOnly);

                switch (befehl)
                {
                    case "variablen":
                    foreach (var file in files)
                    {
                        var variablen = (Dictionary<string, string>)req["Variablen"];
                        var file_content = System.IO.File.ReadAllText(file);
                        foreach (var variable in variablen)
                        {
                            file_content = file_content.Replace(variable.Key, variable.Value.Replace("$APPLICATIONS", Applications.target_path));
                        }
                        System.IO.File.WriteAllText(file, file_content);
                    }
                    break;
                    case "werte":
                    foreach (var file in files)
                    {
                        var werte = (Dictionary<string, object>)req["Werte"];
                        var file_content = System.IO.File.ReadAllText(file);
                        var content = Newtonsoft.Json.Linq.JObject.Parse(file_content);
                        foreach (var wert in werte)
                        {
                            content[wert.Key] = (Newtonsoft.Json.Linq.JToken)wert.Value;
                        }
                        System.IO.File.WriteAllText(file, content.ToString());
                    }
                    break;
                }
            } catch {

            }
        }

        [HttpPost("einrichten/{pfad}")]
        public void Einrichten(string pfad)
        {
            var ordner = pfad.Replace("$APPLICATIONS", Applications.target_path);
            if (ordner.EndsWith("/")) ordner = ordner.Substring(0, ordner.Length-1);
            var files = System.IO.Directory.GetFiles(ordner, "*.json", System.IO.SearchOption.AllDirectories);

            foreach (var file in files)
            {
                for (int i = 0; i < 3; ++i)
                {
                    try {
                        var file_content = System.IO.File.ReadAllText(file);
                        file_content = file_content.Replace("$APPLICATIONS", Applications.target_path);
                        file_content = file_content.Replace("$RESSOURCE", Program.Ressource);
                        file_content = file_content.Replace("$ORDNERDATENSTRUKTUREN", Program.Datenstrukturen);      
                        System.IO.File.WriteAllText(file, file_content);
                        break;
                    } catch {

                    }
                }
            }
        }
    }
}
