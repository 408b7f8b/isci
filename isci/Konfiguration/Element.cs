using System;
using System.Linq;
using System.Collections.Generic;

namespace isci.Konfiguration
{
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
}