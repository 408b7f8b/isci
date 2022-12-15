using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using isci.Beschreibung;

namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Eigenschaften
    {
        public string GetRessource()
        {
            var beschreibung = new Automatisierungsressource();
            beschreibung.Identifikation = Program.Ressource;
            beschreibung.Name = "Automatisierungsressource " + beschreibung.Identifikation;
            beschreibung.Beschreibung = "";

            var info = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture;
            var info2 = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
            var p = System.IO.Path.GetPathRoot(api.Controllers.Applications.target_path);
            var dInfo = new System.IO.DriveInfo(p);

            var network = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
            
            var network_desc = new List<Dictionary<string, object>>();
            foreach (var net in network)
            {
                var desc = new Dictionary<string, object>() {
                    { "Id", net.Id },
                    { "Description", net.Description },
                    { "Type", net.NetworkInterfaceType.ToString() }
                };

                try 
                {
                    var m = (List<System.Net.IPAddress>)net.GetType().GetProperty("Addresses").GetValue(net);
                    desc.Add("Addresses", new List<string>());
                    foreach (var ip in m)
                    {
                        ((List<string>)desc["Addresses"]).Add(ip.ToString());
                    }
                    network_desc.Add(desc);
                } catch {

                }
            }

            beschreibung.Devicebeschreibung = new Dictionary<string, object>() {
                { "OS", info2 },
                { "Architecture", info.ToString() },
                { "Drive space", (dInfo.TotalFreeSpace/1000.0).ToString() },
                { "Free space", (dInfo.AvailableFreeSpace/1000.0).ToString() },
                { "Network", network_desc }
            };            

            var list = General.FoldersInPathList(api.Controllers.Applications.target_path);
            for (int i = 0; i < list.Count; ++i) list[i] = list[i].Substring(api.Controllers.Applications.target_path.Length+1);
            beschreibung.Anwendungen = list;

            return Newtonsoft.Json.JsonConvert.SerializeObject(beschreibung);
        }

        public string GetSystemteil(string Identifikation)
        {
            if (!System.IO.Directory.Exists(Applications.target_path + "/" + Identifikation + "/Beschreibungen"))
            {
                return "{}";
            }

            var systemteil = new Systemteil();

            var modulliste = General.FilesInPathList(Applications.target_path + "/" + Identifikation + "/Beschreibungen");
            foreach (var modul in modulliste)
            {
                var m = Newtonsoft.Json.JsonConvert.DeserializeObject<Modul>(System.IO.File.ReadAllText(Applications.target_path + "/" + Identifikation + "/Beschreibungen/" + modul));
                systemteil.Module.Add(m.Identifikation, m.Typidentifikation);
                systemteil.Dateneinträge.AddRange(m.Dateneinträge);
                systemteil.Ereignisse.AddRange(m.Ereignisse);
                systemteil.Funktionen.AddRange(m.Funktionen);
            }

            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(systemteil);
            return ret;
        }

        [HttpGet("{Identifikation}/{*lol}")]
        public ActionResult<string> Get(string Identifikation, string lol = "")
        {
            switch (Identifikation)
            {
                case "Ressource":
                case "ressource": return GetRessource();
                case "Systemteil":
                case "systemteil": return GetSystemteil(lol);
                default: return "{}";
            }
        }
    }
}