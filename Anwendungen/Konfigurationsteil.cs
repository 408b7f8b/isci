using System;
using System.Linq;
using System.Collections.Generic;
using isci.Konfiguration;

namespace isci.Anwendungen
{
    public class Konfigurationsteil {
        //Automatisierungsressource --> Name Konfiguration -->
        //Identifikation Konfiguration
        public Dictionary<string, Dictionary<string, string>> Konfigurationspakete;
        //Automatisierungsressource --> Konfigurationselemente
        public Dictionary<string, List<Konfigurationselement>> Konfigurationselemente;
    }
}