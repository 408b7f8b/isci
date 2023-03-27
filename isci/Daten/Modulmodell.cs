using System;
using System.Linq;
using isci.Allgemein;

namespace isci.Daten
{
    public class Modulmodell : Datenmodell
    {
        public Modulmodell(string Identifikation, ListeDateneintraege Dateneinträge = null) : base(Identifikation)
        {
            
            var aktiv = new dtBool(true, "aktiv_logik");
            var eingaenge = new dtBool(false, "aktiv_eingaenge");
            var ausgaenge = new dtBool(false, "aktiv_ausgaenge");
            this.Dateneinträge = new ListeDateneintraege(){aktiv, eingaenge, ausgaenge};
        }
    }
}