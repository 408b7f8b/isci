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

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (Double)reader.ReadDouble();
            if (tmp != (Double)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((double)((Double)value));
        }

        public override string Serialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void AusString(System.String s)
        {
            value = Double.Parse(s);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
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
    }
}