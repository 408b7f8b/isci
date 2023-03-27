using System;
using System.Linq;
using System.Collections.Generic;
using isci.Anwendungen;
using isci.Allgemein;
using isci.Daten;

namespace isci.Beschreibung
{
    public class Automatisierungssystem : Header
    {
        public List<string> Automatisierungsressourcen;
        public string Anwendung;
        public Dictionary<string, string> Module;
        public Dictionary<string, string> Modulverteilung;
        public ListeDateneintraege Dateneinträge;
        public List<Ereignis> Ereignisse;
        public List<Funktion> Funktionen;

        public Automatisierungssystem() : base()
        {

        }

        public Automatisierungssystem(string pfad) : base()
        {
            var list = System.IO.Directory.GetFiles(pfad, "*", System.IO.SearchOption.TopDirectoryOnly).ToList<string>();

            foreach (var file in list)
            {
                var file_ = System.IO.File.ReadAllText(file);
                var teil = Newtonsoft.Json.JsonConvert.DeserializeObject<Systemteil>(file_);
                //Automatisierungsressourcen.Add(teil.Ressource);
                foreach (var m in teil.Module)
                {
                    Module.Add(m.Key, m.Value);
                    //Modulverteilung.Add(m.Key, teil.Ressource);
                }
                Dateneinträge.AddRange(teil.Dateneinträge);
                Ereignisse.AddRange(teil.Ereignisse);
                Funktionen.AddRange(teil.Funktionen);
            }
        }
    }
}