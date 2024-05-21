using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;
using isci.Konfiguration;
using isci.Daten;

namespace isci.Anwendungen
{
    public class Basis : Header {
        public List<string> ReferenzierteModelle;
        
        /*//Stufe --> Konfigurationsteil
        public Dictionary<string, Konfigurationsteil> Konfigurationsteile;
        
        //Stufe --> Modelle
        public Dictionary<string, Datamodel> Datenmodelle;
        public Dictionary<string, List<Funktion>> Funktionen;
        public Dictionary<string, List<Ereignis>> Ereignisse;*/

        //Automatisierungsressource --> Name Konfiguration -->
        //Identifikation Konfiguration
        public Dictionary<string, List<string>> Konfigurationspakete;
        //Automatisierungsressource --> Konfigurationselemente
        public Dictionary<string, List<Konfigurationselement>> Konfigurationselemente;
        
        //Modelle
        public List<Datenmodell> Datenmodelle;
        public List<Funktion> Funktionen;
        public List<Ereignis> Ereignisse;

        public Dictionary<string, Ausführungsmodell> Ausführungsmodelle;

        //noch ungeprüft
        public void KartierungAnwenden(Dictionary<string, string> Kartierung)
        {
            var Konfigurationspakete_neu = new Dictionary<string, List<string>>();
            var Konfigurationselemente_neu = new Dictionary<string, List<Konfigurationselement>>();
            var Ausführungsmodelle_neu = new Dictionary<string, Ausführungsmodell>();
            foreach (var Verknuepfung in Kartierung)
            {
                if (Konfigurationspakete.ContainsKey(Verknuepfung.Key)) {
                    if (!Konfigurationspakete_neu.ContainsKey(Verknuepfung.Value))
                    {
                        Konfigurationspakete_neu.Add(Verknuepfung.Value, new List<string>());
                    }

                    Konfigurationspakete_neu[Verknuepfung.Value].AddRange(Konfigurationspakete[Verknuepfung.Key]);
                }

                if (Konfigurationselemente.ContainsKey(Verknuepfung.Key))
                {
                    if (!Konfigurationselemente_neu.ContainsKey(Verknuepfung.Value))
                    {
                        Konfigurationselemente.Add(Verknuepfung.Value, new List<Konfigurationselement>());
                    }

                    Konfigurationselemente_neu[Verknuepfung.Value].AddRange(Konfigurationselemente[Verknuepfung.Key]);
                }

                if (Ausführungsmodelle.ContainsKey(Verknuepfung.Key))
                {
                    if (!Ausführungsmodelle_neu.ContainsKey(Verknuepfung.Value))
                    {
                        Ausführungsmodelle_neu.Add(Verknuepfung.Value, new Ausführungsmodell());
                    }

                    Ausführungsmodelle_neu[Verknuepfung.Value] = new Ausführungsmodell(new List<Ausführungsmodell>(){ Ausführungsmodelle_neu[Verknuepfung.Value], Ausführungsmodelle[Verknuepfung.Key] });
                }
            }
            Konfigurationspakete = Konfigurationspakete_neu;
            Konfigurationselemente = Konfigurationselemente_neu;
            Ausführungsmodelle = Ausführungsmodelle_neu;
        }
    }
}