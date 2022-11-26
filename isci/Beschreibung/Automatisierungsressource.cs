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
        public List<Schnittstelle> Schnittstellen;
    }
}