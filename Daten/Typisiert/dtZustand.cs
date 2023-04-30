using System;
using System.Linq;

namespace isci.Daten
{
    public class dtZustand : dtInt32
    {
        public dtZustand(String ordnerDatenstruktur) : base(0, "Zustand", ordnerDatenstruktur + "/Zustand")
        {

        }
    }
}