using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Ausführungsschritt
    {
        public string Modulidentifikation;
        public object Parametrierung;
        public int Folgezustand = -1;
    }
}