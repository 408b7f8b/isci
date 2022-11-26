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
        public Dictionary<string, Dictionary<string, string>> Konfigurationspakete;
        //Automatisierungsressource --> Konfigurationselemente
        public Dictionary<string, List<Konfigurationselement>> Konfigurationselemente;
        
        //Modelle
        public List<Datenmodell> Datenmodelle;
        public List<Funktion> Funktionen;
        public List<Ereignis> Ereignisse;
    }
}