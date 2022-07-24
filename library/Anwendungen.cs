using System;
using System.Linq;
using library;
using System.Collections.Generic;

namespace Anwendungen
{
    public class Funktion : Modell {
	    public List<string> Ziele;
    }

    public class Ereignis : Modell {
        public List<string> Elemente;
    }

    public class Konfigurationselement {
        public string Typ;
        public object Vorgang;

        public class Datei
        {
            public string Quelle;
            public string Name;
            public string Ordner;
        }

        public class Dienst
        {
            public string Name;
            public string Ziel;
            public string Operation;
            public string Arbeitspfad;
        }
        
        public class Parameter
        {
            public string Ordner;
            public Dictionary<string, string> Variablen;
        }
    }

    public class Konfigurationspaket : Modell {
        public List<Konfigurationselement> Elemente;
    }

    public class Konfigurationsteil {
        //Automatisierungsressource --> Name Konfiguration -->
        //Identifikation Konfiguration
        public Dictionary<string, Dictionary<string, string>> Konfigurationspakete;
        //Automatisierungsressource --> Konfigurationselemente
        public Dictionary<string, List<Konfigurationselement>> Konfigurationselemente;
    }

    public class Basis : Modell {
        public List<string> ReferenzierteModelle;
        
        /*//Stufe --> Konfigurationsteil
        public Dictionary<string, Konfigurationsteil> Konfigurationsteile;
        
        //Stufe --> Modelle
        public Dictionary<string, Datamodel> Datenmodelle;
        public Dictionary<string, List<Funktion>> Funktionen;
        public Dictionary<string, List<Ereignis>> Ereignisse;*/

        //Automatisierungsressource --> Name Konfiguration -->
        //Identifikation Konfiguration
        public Dictionary<string, Dictionary<string, string>> Konfigurationspakete;
        //Automatisierungsressource --> Konfigurationselemente
        public Dictionary<string, List<Konfigurationselement>> Konfigurationselemente;
        
        //Modelle
        public List<Datamodel> Datenmodelle;
        public List<Funktion> Funktionen;
        public List<Ereignis> Ereignisse;
    }

    public class Instanz : Basis {
        //Identifikation Basis --> Automatisierungsressourcepseudonym
        //--> Automatisierungsressource
        public Dictionary<string, Dictionary<string, string>> Ressourcenkartierung;
        //Identifikation Basis --> aktivierte Stufen
        //public Dictionary<string, List<string>> AktiveStufen;
        //public Dictionary<string, string> SchnittstellenUndIhreRessourcen;
    }
}