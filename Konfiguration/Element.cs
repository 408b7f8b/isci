using System;
using System.Linq;
using System.Collections.Generic;

namespace isci.Konfiguration
{
    public class Konfigurationselement {
        public string typ;
        public Vorgang vorgang;
    }

    public abstract class Vorgang
    {

    }

    public class Datei : Vorgang
    {
        public string Quelle;
        public string Name;
        public string Ordner;
    }

    public class Dienst : Vorgang
    {
        public string Name;
        public string Ziel;
        public string Operation;
        public string Arbeitspfad;
    }
    
    public class Parameter : Vorgang
    {
        public string Ordner;
        public Dictionary<string, string> Variablen;
    }
}