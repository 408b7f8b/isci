using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Header
    {
        public string Identifikation;
        public string Name;
        public string Beschreibung;
        public string Typ;

        public Header()
        {
            this.Typ = this.GetType().FullName;
        }
        
        public Header(string Identifikation, string Name = "", string Beschreibung = "", string Typ = "")
        {
            if (Typ == "") this.Typ = this.GetType().FullName;
            this.Beschreibung = Beschreibung;
            this.Name = Name;
            this.Identifikation = Identifikation;
        }

        public void Speichern(string pfad = "")
        {
            try {
                var ser = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                if (pfad == "" || pfad.EndsWith("/")) pfad += Identifikation + ".json";
                if (pfad.Contains("/")) Helfer.OrdnerPruefenErstellen(System.IO.Path.GetDirectoryName(pfad));
                System.IO.File.WriteAllText(pfad, ser);
            } catch {

            }            
        }
    }
}