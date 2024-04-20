using System;
using System.Collections.Generic;
using System.Linq;

namespace isci.Allgemein
{
    public class Ausführungsmodell : System.Collections.Generic.SortedDictionary<uint, Ausführungsschritt>
    {
        public Daten.dtZustand Zustand;

        public Ausführungsmodell() : base()
        {

        }

        public Ausführungsmodell(Parameter konfiguration, Daten.dtZustand zustand) : base()
        {
            Newtonsoft.Json.Linq.JObject geparst = null;

            try
            {
                var datei = (konfiguration.OrdnerAnwendungen + "/" + konfiguration.Anwendung + "/Ausführungsmodell.json").Replace("//", "/");
                geparst = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(datei));
            }
            catch (System.Exception e)
            {
                Logger.Fatal("Ausführungsmodell ist nicht vorhanden oder konnte nicht gelesen werden: " + e.Message);
                System.Environment.Exit(-1);
            }

            try
            {
                foreach (Newtonsoft.Json.Linq.JProperty child in geparst.Properties())
                {
                    var identifikation_ = child.Value.SelectToken("Modulidentifikation").ToObject<string>();
                    if (identifikation_ != konfiguration.Identifikation) continue;
                    var index = uint.Parse(child.Name);
                    var schritt = child.Value.ToObject<Ausführungsschritt>();
                    if (index == geparst.Properties().Count()-1)
                    {
                        if (schritt.Folgezustand != 0) schritt.Folgezustand = 0;
                    }
                    Add(index, schritt);
                }
            }
            catch (System.Exception e)
            {
                Logger.Fatal("JSON-Verarbeitung des Ausführungsmodells fehlgeschlagen für die Modulinstanz: " + konfiguration.Identifikation + ", " + e.Message);
                System.Environment.Exit(-1);
            }

            if (this.Count() == 0)
            {
                Logger.Fatal("Kein Eintrag im Ausführungsmodell für die Modulinstanz: " + konfiguration.Identifikation);
                System.Environment.Exit(-1);
            }

            this.Zustand = zustand;

            zustand.WertAusSpeicherLesen();
            if (!geparst.ContainsKey(zustand.WertSerialisieren()))
            {
                Logger.Information("Zustandswert war außerhalb des Ausführungsmodell. Wird auf 0 zurückgesetzt.");
                zustand.Wert = 0;
                zustand.WertInSpeicherSchreiben();
            }
        }

        public Ausführungsmodell(List<Ausführungsmodell> Ausführungsmodelle) : base()
        {
            uint i = 0;
            int letzterCount = -1;
            while(letzterCount != this.Count())
            {
                letzterCount = this.Count();
                foreach (var modell in Ausführungsmodelle)
                {
                    if (modell.ContainsKey(i)) this.Add((uint)this.Count(), modell[i]);
                }
                ++i;
            }
        }

        public static Ausführungsmodell ausDatei(string modell)
        {
            try
            {
                Logger.Debug("JSON-Parsing für Ausführungsmodell: " + modell);
                var inhalt = System.IO.File.ReadAllText(modell);
                var ret = Newtonsoft.Json.JsonConvert.DeserializeObject<Ausführungsmodell>(inhalt);
                return ret;
            }
            catch (System.Exception e)
            {
                Logger.Fehler("Ausnahme beim JSON-Parsing des Ausführungsmodells: " + e.Message);
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
                    Logger.Information("Ausführungsmodell: Modulinstanz " + Modulidentifikation + " als aktiv eingetragen bei Zustand: " + Schritt.Key);
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
                    Logger.Information("Ausführungsmodell: Modulinstanz " + Modulidentifikation + " als aktiv eingetragen bei Zustand: " + Schritt.Key);
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

        public void Folgezustand()
        {
            if (this[Zustand.Wert].Folgezustand == -1)
            {
                Zustand.Wert++;
            }
            else
            {
                Zustand.Wert = (ushort)this[Zustand.Wert].Folgezustand;
            }
        }

        public void Folgezustand(Daten.dtZustand Zustand)
        {
            if (this[Zustand.Wert].Folgezustand == -1)
            {
                Zustand.Wert++;
            }
            else
            {
                Zustand.Wert = (ushort)this[Zustand.Wert].Folgezustand;
            }
        }
    }
}