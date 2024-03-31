using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;

namespace isci.Beschreibung
{
    public class Automatisierungsressource : Header
    {
        public string Handler;
        public Dictionary<string, object> Devicebeschreibung;
        public List<string> Anwendungen;
        public Dictionary<string, Systemteil> Systemteile;
    }

    public class Automatisierungsressource_ : Header
    {
        public List<string> Anwendungen;
        public Dictionary<string, List<Modul>> AnwendungenUndModule;
    }
}