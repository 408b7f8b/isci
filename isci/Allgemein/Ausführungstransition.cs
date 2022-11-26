using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Ausführungstransition
    {
        public int Eingangszustand;
        public int Ausgangszustand;

        public Ausführungstransition() { }

        public Ausführungstransition(int Eingangszustand, int Ausgangszustand)
        {
            this.Eingangszustand = Eingangszustand;
            this.Ausgangszustand = Ausgangszustand;
        }
    }
}