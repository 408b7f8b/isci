using System;
using System.Linq;

namespace isci.Daten
{
    public class dtZustand : dtUInt16
    {
        private new EinheitenKodierung Einheit;
        public dtZustand(String ordnerDatenstruktur) : base(0, "Zustand", ordnerDatenstruktur + "/Zustand")
        {

        }
    }
}