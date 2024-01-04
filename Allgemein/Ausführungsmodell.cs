using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Ausführungsmodell : System.Collections.Generic.Dictionary<uint, Ausführungsschritt>
    {
        public Ausführungsmodell() : base()
        {
            
        }

        public Ausführungsmodell(Parameter konfiguration) : base()
        {
            var datei = (konfiguration.OrdnerAnwendungen + "/" + konfiguration.Anwendung + "/Ausführungsmodell.json").Replace("//", "/");
            var geparst = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(datei));

            foreach (Newtonsoft.Json.Linq.JProperty child in geparst.Properties())
            {
                var identifikation_ = child.Value.SelectToken("Modulidentifikation").ToObject<string>();
                if (identifikation_ != konfiguration.Identifikation) continue;
                var index = uint.Parse(child.Name);
                Add(index, child.Value.ToObject<Ausführungsschritt>());
            }
        }

        public static Ausführungsmodell ausDatei(string modell)
        {
            try {
                var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<Ausführungsmodell>(modell);
                return ret;
            } catch {
                return null;
            }
        }

        public System.Collections.Generic.List<uint> AusführungsschritteNachSchlüssel(string Modulidentifikation)
        {
            var ret = new System.Collections.Generic.List<uint>();

            foreach (var Schritt in this)
            {
                if (Schritt.Value.Modulidentifikation == Modulidentifikation)
                {
                    ret.Add(Schritt.Key);
                }
            }

            return ret;
        }

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<uint, Ausführungsschritt>> AusführungsschritteNachWert(string Modulidentifikation)
        {
            var ret = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<uint, Ausführungsschritt>>();

            foreach (var Schritt in this)
            {
                if (Schritt.Value.Modulidentifikation == Modulidentifikation)
                {
                    ret.Add(Schritt);
                }
            }

            return ret;            
        }
    }
}