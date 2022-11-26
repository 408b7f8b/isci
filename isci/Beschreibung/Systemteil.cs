using System;
using System.Linq;
using System.Collections.Generic;
using isci.Anwendungen;
using isci.Allgemein;
using isci.Daten;

namespace isci.Beschreibung
{
    public class Systemteil : Header
    {
        public string Ressource;
        public Dictionary<string, string> Module;
        public ListeDateneintraege Dateneinträge;
        public List<Ereignis> Ereignisse;
        public List<Funktion> Funktionen;
    }
}