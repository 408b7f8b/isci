using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;

namespace isci.Beschreibung
{
    public class Automatisierungsressource : Header
    {
        public string Handler;
        public Dictionary<string, object> Devicebeschreibung;
        public List<string> Anwendungen;
        public Dictionary<string, Systemteil> Systemteile;
    }

    public class Automatisierungsressource_ : Header
    {
        public List<string> Anwendungen;
        public Dictionary<string, List<Modul>> AnwendungenUndModule;

        public Automatisierungsressource_(string pfad)
        {
            var anwendungenOrdner = System.IO.Directory.GetDirectories(pfad);

            Anwendungen = new List<string>();
            AnwendungenUndModule = new Dictionary<string, List<Modul>>();

            foreach (var anwendung in anwendungenOrdner)
            {
                var Name = System.IO.Path.GetDirectoryName(anwendung);
                Anwendungen.Add(Name);
                AnwendungenUndModule.Add(Name, new List<Modul>());

                var module = System.IO.Directory.GetFiles(anwendung + "/Beschreibungen");

                foreach (var beschreibung in module)
                {
                    var serialisiert = System.IO.File.ReadAllText(beschreibung);
                    var modul = Newtonsoft.Json.JsonConvert.DeserializeObject<Modul>(serialisiert);
                    AnwendungenUndModule[Name].Add(modul);
                }

                //Ausführungsreihenfolge
                //Metainformationen
                //Loggingblabla
            }
        }
    }
}