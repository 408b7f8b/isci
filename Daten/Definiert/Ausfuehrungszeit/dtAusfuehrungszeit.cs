using System;
using System.Collections.Generic;
using System.Linq;

namespace isci.Daten
{
    public class dtAusfuehrungsanalyse : dtObjekt
    {
#pragma warning disable CS0649
        private dtDouble LetzteDauer;
        private dtDouble MaximaleDauer;
        private dtDouble SchnittDauer;
#pragma warning restore CS0649
        private List<double> Schnittbildung;
        public uint AnzahlElementeSchnittbildung;
        private uint PositionSchnittbildung;

        public dtAusfuehrungsanalyse() : base("Ausfuehrungsanalyse")
        {
            Schnittbildung = new List<double>();
            AnzahlElementeSchnittbildung = 100;

            /* LetzteDauer = new dtDouble(0.0, "LetzteDauer");
            MaximaleDauer = new dtDouble(0.0, "MaximaleDauer");
            SchnittDauer = new dtDouble(0.0, "SchnittDauer"); */

            ElementeLaufzeit = new List<Dateneintrag>()
            {
                LetzteDauer, MaximaleDauer, SchnittDauer
            };

            Elemente = new List<string>()
            {
                LetzteDauer.Identifikation, MaximaleDauer.Identifikation, SchnittDauer.Identifikation
            };
        }

        private long curr_ticks;

        public void MessungStart()
        {
            curr_ticks = System.DateTime.Now.Ticks;
        }

        public void MessungEnde()
        {
            var ticks_span = System.DateTime.Now.Ticks - curr_ticks;
            LetzteDauer.Wert = (double)ticks_span / System.TimeSpan.TicksPerMillisecond;

            if (MaximaleDauer.Wert < LetzteDauer.Wert) MaximaleDauer.Wert = LetzteDauer.Wert;

            if (Schnittbildung.Count < AnzahlElementeSchnittbildung)
            {
                Schnittbildung.Add(LetzteDauer.Wert);
                PositionSchnittbildung = (uint)Schnittbildung.Count();
            } else {
                Schnittbildung[(int)PositionSchnittbildung] = LetzteDauer.Wert;
                ++PositionSchnittbildung;
                if (PositionSchnittbildung == AnzahlElementeSchnittbildung) PositionSchnittbildung = 0;
            }
            SchnittDauer.Wert = Schnittbildung.Average();
        }

    }
}