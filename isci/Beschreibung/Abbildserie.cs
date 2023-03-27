using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;

namespace isci.Beschreibung
{
    public class Abbildserie
    {
        public string Automatisierungsressource;
        public string Anwendung;
        public List<System.DateTime> Zeitstempel;
        public Dictionary<string, List<object>> WerteNachDateneinträgen;
    }
}