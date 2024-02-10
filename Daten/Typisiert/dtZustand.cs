using System;
using System.Linq;

namespace isci.Daten
{
    public class dtZustand : dtUInt16
    {
        private new EinheitenKodierung Einheit;
        public dtZustand() : base(0, "Zustand")
        {

        }

        public static dtZustand operator ++(dtZustand element)
        {
            element.Wert++;
            return element;
        }
    }
}