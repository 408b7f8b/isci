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

        //Ressource --> Modulidentifikation --> Liste Transitionen
        public Ausführungsmodell ausführungsmodell;
    }
}