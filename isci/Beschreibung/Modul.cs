using System;
using System.Linq;
using System.Collections.Generic;
using isci.Anwendungen;
using isci.Allgemein;
using isci.Daten;

namespace isci.Beschreibung
{
    public class Modul : Header
    {
        public string Typidentifikation;
        public ListeDateneintraege Datenfelder;
        public List<Ereignis> Ereignisse;
        public List<Funktion> Funktionen;

        public Modul(string Identifikation, string Typidentifikation, ListeDateneintraege Datenfelder = null, List<Ereignis> Ereignisse = null, List<Funktion> Funktionen = null) : base(Identifikation) {
            this.Typidentifikation = Typidentifikation;
            this.Datenfelder = (Datenfelder == null ? new ListeDateneintraege() : Datenfelder);
            this.Ereignisse = (Ereignisse == null ? new List<Ereignis>() : Ereignisse);
            this.Funktionen = (Funktionen == null ? new List<Funktion>() : Funktionen);
        }
    }
}