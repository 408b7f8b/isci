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
        public Dictionary<string, Modul> Module;
        public Dictionary<string, string> Modulverteilung;
        public ListeDateneintraege Dateneinträge;
        public List<Datenmodell> Datenmodelle;
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

        public Automatisierungssystem(string Anwendung, List<string> Automatisierungsressourcen, string Pfad_Beschreibungen, string Pfad_Datenmodelle, string Pfad_Ereignismodelle, string Pfad_Funktionsmodelle, string Pfad_Schnittstellen)
        {
            this.Automatisierungsressourcen = Automatisierungsressourcen;
            this.Anwendung = Anwendung;

            Module = new Dictionary<string, Modul>();
            var dateien = System.IO.Directory.GetFiles(Pfad_Beschreibungen, "*.json");
            foreach (var datei in dateien)
            {
                var modul_serialisiert = System.IO.File.ReadAllText(datei);
                var modul = Newtonsoft.Json.JsonConvert.DeserializeObject<Modul>(modul_serialisiert);
                Module.Add(modul.Identifikation, modul);
            }

            Datenmodelle = new List<Datenmodell>();
            dateien = System.IO.Directory.GetFiles(Pfad_Datenmodelle, "*.json");
            foreach (var datei in dateien)
            {
                var datenmodell_serialisiert = System.IO.File.ReadAllText(datei);
                var datenmodell = Newtonsoft.Json.JsonConvert.DeserializeObject<Datenmodell>(datenmodell_serialisiert);
                Datenmodelle.Add(datenmodell);
            }
        }
    }
}