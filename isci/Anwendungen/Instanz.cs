using System;
using System.Linq;
using System.Collections.Generic;
using isci;
using isci.Allgemein;

namespace isci.Anwendungen
{
    public class Instanz : Basis {
        //Identifikation Basis --> Automatisierungsressourcepseudonym
        //--> Automatisierungsressource
        public Dictionary<string, Dictionary<string, string>> Ressourcenkartierung;
        //Identifikation Basis --> aktivierte Stufen
        //public Dictionary<string, List<string>> AktiveStufen;
        //public Dictionary<string, string> SchnittstellenUndIhreRessourcen;

        //Ressource --> Modulidentifikation --> Liste Transitionen
        public Dictionary<string, Dictionary<string, List<Ausführungstransition>>> Ausführungstransitionen;
    }
}