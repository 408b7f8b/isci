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
        public ListeDateneintraege Dateneinträge;
        public List<Ereignis> Ereignisse;
        public List<Funktion> Funktionen;
        public string Automatisierungsressource;

        public Modul(string Identifikation, string Typidentifikation, Datenmodell datenmodell, string automatisierungsressource = "", List<Ereignis> Ereignisse = null, List<Funktion> Funktionen = null) : base(Identifikation) {
            this.Typidentifikation = Typidentifikation;
            this.Dateneinträge = (datenmodell == null ? new ListeDateneintraege() : datenmodell.Dateneinträge);
            this.Automatisierungsressource = automatisierungsressource;
            this.Ereignisse = (Ereignisse == null ? new List<Ereignis>() : Ereignisse);
            this.Funktionen = (Funktionen == null ? new List<Funktion>() : Funktionen);
        }

        public Modul(string Identifikation, string Typidentifikation, List<Datenmodell> datenmodelle, string automatisierungsressource = "", List<Ereignis> Ereignisse = null, List<Funktion> Funktionen = null) : base(Identifikation)
        {
            this.Typidentifikation = Typidentifikation;
            this.Dateneinträge = new ListeDateneintraege();
            foreach (var modell in datenmodelle)
            {
                this.Dateneinträge.AddRange(modell.Dateneinträge);
            }
            this.Automatisierungsressource = automatisierungsressource;
            this.Ereignisse = (Ereignisse == null ? new List<Ereignis>() : Ereignisse);
            this.Funktionen = (Funktionen == null ? new List<Funktion>() : Funktionen);
        }

        public Modul(string Identifikation, string Typidentifikation, ListeDateneintraege Dateneinträge = null, string automatisierungsressource = "", List<Ereignis> Ereignisse = null, List<Funktion> Funktionen = null) : base(Identifikation) {
            this.Typidentifikation = Typidentifikation;
            this.Dateneinträge = (Dateneinträge == null ? new ListeDateneintraege() : Dateneinträge);
            this.Automatisierungsressource = automatisierungsressource;
            this.Ereignisse = (Ereignisse == null ? new List<Ereignis>() : Ereignisse);
            this.Funktionen = (Funktionen == null ? new List<Funktion>() : Funktionen);
        }

        public void Speichern(Parameter parameter)
        {
            Speichern(parameter.OrdnerBeschreibungen + "/" + parameter.Identifikation + ".json");
        }
    }
}