using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;

namespace isci.Konfiguration
{
    public class Konfigurationspaket : Header {
        public List<Konfigurationselement> Elemente;
    }

    public class Konfigurationsteil {
        //Automatisierungsressource --> Name Konfiguration -->
        //Identifikation Konfiguration
        public Dictionary<string, Dictionary<string, string>> Konfigurationspakete;
        //Automatisierungsressource --> Konfigurationselemente
        public Dictionary<string, List<Konfigurationselement>> Konfigurationselemente;
    }
}