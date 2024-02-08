using System;
using System.Linq;

namespace isci.Daten
{
    public class dtFloat : Dateneintrag
    {
        //public new float Wert;

        public dtFloat(float Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Float;
            this.Wert = Wert;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (float)reader.ReadDouble();
            if (tmp != (float)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((double)((float)Wert));
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = float.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<float>();
        }

        public static bool operator ==(dtFloat left, dtFloat right)
        {
            return (float)left.Wert == (float)right.Wert;
        }

        public static bool operator !=(dtFloat left, dtFloat right)
        {
            return (float)left.Wert != (float)right.Wert;
        }

        public static bool operator ==(dtFloat left, float right)
        {
            return (float)left.Wert == right;
        }

        public static bool operator !=(dtFloat left, float right)
        {
            return (float)left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtFloat other)
            {
                return Wert == other.Wert;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Wert.GetHashCode();
        }
    }
}