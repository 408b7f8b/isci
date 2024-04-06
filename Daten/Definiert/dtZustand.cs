using System;
using System.Linq;

namespace isci.Daten
{
    public class dtZustand : dtUInt16
    {
#pragma warning disable CS0169
        private new EinheitenKodierung Einheit;
#pragma warning restore CS0169
        public dtZustand() : base(0, "Zustand")
        {

        }

        public static dtZustand operator ++(dtZustand element)
        {
            element.Wert++;
            return element;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            while (reader.BaseStream.Length != sizeof(UInt16)) {}
            var tmp = reader.ReadUInt16();
            if (tmp != Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }
    }
}