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

    public class Ausführungsschritt
    {
        public string Modulidentifikation;
        public object Parametrierung;
    }

    public class Ausführungsmodell : System.Collections.Generic.Dictionary<uint, Ausführungsschritt>
    {
        public System.Collections.Generic.List<uint> AusführungsschritteNachSchlüssel(string Modulidentifikation)
        {
            var ret = new System.Collections.Generic.List<uint>();

            foreach (var Schritt in this)
            {
                if (Schritt.Value.Modulidentifikation == Modulidentifikation)
                {
                    ret.Add(Schritt.Key);
                }
            }

            return ret;
        }

        public System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<uint, Ausführungsschritt>> AusführungsschritteNachWert(string Modulidentifikation)
        {
            var ret = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<uint, Ausführungsschritt>>();

            foreach (var Schritt in this)
            {
                if (Schritt.Value.Modulidentifikation == Modulidentifikation)
                {
                    ret.Add(Schritt);
                }
            }

            return ret;            
        }
    }
}