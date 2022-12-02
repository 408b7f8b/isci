using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;
using System.Datetime;

namespace isci.Beschreibung
{
    public class Abbildserie
    {
        public string Automatisierungsressource;
        public string Anwendung;
        public List<Datetime> Zeitstempel;
        public Dictionary<string, List<object>> WerteNachDateneinträgen;
    }
}