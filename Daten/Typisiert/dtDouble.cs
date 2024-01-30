using System;
using System.Linq;

namespace isci.Daten
{
    public class dtDouble : Dateneintrag
    {
        //public new Double value;

        public dtDouble(Double value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Double;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (Double)reader.ReadDouble();
            if (tmp != (Double)value)
            {
                value = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((double)((Double)value));
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = Double.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<Double>();
        }

        public static bool operator ==(dtDouble left, dtDouble right)
        {
            return (Double)left.value == (Double)right.value;
        }

        public static bool operator !=(dtDouble left, dtDouble right)
        {
            return (Double)left.value != (Double)right.value;
        }

        public static bool operator ==(dtDouble left, Double right)
        {
            return (Double)left.value == right;
        }

        public static bool operator !=(dtDouble left, Double right)
        {
            return (Double)left.value != right;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is dtDouble other)
            {
                return value == other.value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}