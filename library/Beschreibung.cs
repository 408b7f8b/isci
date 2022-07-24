using System;
using System.Linq;
using System.Collections.Generic;

namespace Beschreibung
{
    public class Automatisierungssystem : library.Modell
    {
        public List<string> Ressourcen;
        public Dictionary<string, string> Module;
        public Dictionary<string, string> Modulverteilung;
        public library.FieldList Datenfelder;
        public List<library.Ereignis> Ereignisse;
        public List<library.Funktion> Funktionen;

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
                Ressourcen.Add(teil.Ressource);
                foreach (var m in teil.Module)
                {
                    Module.Add(m.Key, m.Value);
                    Modulverteilung.Add(m.Key, teil.Ressource);
                }
                Datenfelder.AddRange(teil.Datenfelder);
                Ereignisse.AddRange(teil.Ereignisse);
                Funktionen.AddRange(teil.Funktionen);
            }
        }
    }
    public class Systemteil : library.Modell
    {
        public string Ressource;
        public Dictionary<string, string> Module;
        public library.FieldList Datenfelder;
        public List<library.Ereignis> Ereignisse;
        public List<library.Funktion> Funktionen;
    }
    public class Modul : library.Modell
    {
        public string Typidentifikation;
        public library.FieldList Datenfelder;
        public List<library.Ereignis> Ereignisse;
        public List<library.Funktion> Funktionen;
    }
    public class Ressource : library.Modell
    {
        public string Handler;
        public Dictionary<string, object> Devicebeschreibung;
        public List<string> Anwendungen;
    }
}