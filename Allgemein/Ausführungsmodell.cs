using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Ausführungsmodell : System.Collections.Generic.SortedDictionary<uint, Ausführungsschritt>
    {
        public Daten.dtZustand Zustand;

        public Ausführungsmodell() : base()
        {
            
        }

        public Ausführungsmodell(Parameter konfiguration, Daten.dtZustand zustand = null) : base()
        {
            Newtonsoft.Json.Linq.JObject geparst = null;

            try {
                var datei = (konfiguration.OrdnerAnwendungen + "/" + konfiguration.Anwendung + "/Ausführungsmodell.json").Replace("//", "/");
                geparst = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(datei));
            } catch (System.Exception e) {
                Logger.Loggen(Logger.Qualität.ERROR, "Ausführungsmodell ist nicht vorhanden oder konnte nicht gelesen werden: " + e.Message);
                System.Environment.Exit(-1);
            }

            try
            {
                foreach (Newtonsoft.Json.Linq.JProperty child in geparst.Properties())
                {
                    var identifikation_ = child.Value.SelectToken("Modulidentifikation").ToObject<string>();
                    if (identifikation_ != konfiguration.Identifikation) continue;
                    var index = uint.Parse(child.Name);
                    Add(index, child.Value.ToObject<Ausführungsschritt>());
                }
            }
            catch (System.Exception e)
            {
                Logger.Loggen(Logger.Qualität.ERROR, "JSON-Verarbeitung des Ausführungsmodells fehlgeschlagen für die Modulinstanz: " + konfiguration.Identifikation + ", " + e.Message);
                System.Environment.Exit(-1);
            }

            if (this.Count() == 0)
            {
                Logger.Loggen(Logger.Qualität.ERROR, "Kein Eintrag im Ausführungsmodell für die Modulinstanz: " + konfiguration.Identifikation);
                System.Environment.Exit(-1);
            }

            if (zustand != null) this.Zustand = zustand;
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

        public bool AktuellerZustandModulAktivieren()
        {
            return ContainsKey(Zustand.Wert);
        }

        public bool AktuellerZustandModulAktivieren(Daten.dtZustand Zustand)
        {
            return ContainsKey(Zustand.Wert);
        }

        public object ParameterAktuellerZustand()
        {
            return this[Zustand.Wert].Parametrierung;
        }

        public object ParameterAktuellerZustand(Daten.dtZustand Zustand)
        {
            return this[Zustand.Wert].Parametrierung;
        }
    }
}