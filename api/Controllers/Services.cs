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
    public class Services
    {
        public static string target_path;

        [HttpPost("anforderung")]
        public void Anforderung([FromBody]string request)
        {
            try
            {
                var req = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
                var befehl = req["Cmd"];
                switch (befehl)
                {
                    case "create": Create(req["Identifikation"], req["Pfad"], req["Arbeitspfad"]);    break;
                    case "reload": Reload(); break;
                    case "enable": Enable(req["Identifikation"]); break;
                    case "disable": Disable(req["Identifikation"]); break;
                    case "start": Start(req["Identifikation"]); break;
                    case "stop": Stop(req["Identifikation"]); break;
                    case "restart": Restart(req["Identifikation"]); break;
                }
            } catch {

            }
        }

        [HttpPost("create/{Identifikation}")]
        public void Create(string Identifikation, [FromQuery]string pfad, [FromQuery]string arbeitspfad)
        {
            if (pfad.Contains("$APPLICATIONS")) pfad = pfad.Replace("$APPLICATIONS", Applications.target_path);
            var ordner = System.IO.Path.GetDirectoryName(pfad);
            if (!System.IO.Directory.Exists(ordner)) System.IO.Directory.CreateDirectory(ordner);
            if (arbeitspfad.Contains("$APPLICATIONS")) arbeitspfad = arbeitspfad.Replace("$APPLICATIONS", Applications.target_path);
            if (arbeitspfad.EndsWith("/")) arbeitspfad = arbeitspfad.Substring(0, arbeitspfad.Length-1);

            string Beschreibung = $"[Unit]\n" +
            $"Description={Identifikation}\n\n" +
            "[Service]\n" +
            $"WorkingDirectory={arbeitspfad}\n" +
            $"ExecStart={pfad}\n" +
            "Restart=always\n\n" +
            "[Install]\n" +
            "WantedBy=default.target";

            System.IO.File.WriteAllText(target_path + "/" + Identifikation + ".service", Beschreibung);

            General.Action("/bin/sh", $"-c \"chmod +x {target_path}/{Identifikation}.service\"");
        }

        [HttpPost("reload")]
        public void Reload()
        {
            General.Action("/bin/sh", $"-c \"systemctl --user daemon-reload\"");
        }

        [HttpPost("enable/{Identifikation}")]
        public void Enable(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user enable --now {Identifikation}\"");
        }

        [HttpPost("disable/{Identifikation}")]
        public void Disable(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user disable --now {Identifikation}\"");
        }

        [HttpPost("start/{Identifikation}")]
        public void Start(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user start --now {Identifikation}\"");
        }

        [HttpPost("stop/{Identifikation}")]
        public void Stop(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user stop --now {Identifikation}\"");
        }

        [HttpPost("restart/{Identifikation}")]
        public void Restart(string Identifikation)
        {
            General.Action("/bin/sh", $"-c \"systemctl --user restart --now {Identifikation}\"");
        }
    }
}
