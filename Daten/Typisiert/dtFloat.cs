using System;
using System.Linq;

namespace isci.Daten
{
    public class UniList<T> : Dateneintrag
    {
        public UniList(System.Collections.Generic.List<T> values)
        {

        }
    }

    public class dtFloat : Dateneintrag
    {
        //public new float value;

        public dtFloat(float value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Float;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (float)reader.ReadDouble();
            if (tmp != (float)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((double)((float)value));
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = float.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<float>();
        }

        public static bool operator ==(dtFloat left, dtFloat right)
        {
            return (float)left.value == (float)right.value;
        }

        public static bool operator !=(dtFloat left, dtFloat right)
        {
            return (float)left.value != (float)right.value;
        }

        public static bool operator ==(dtFloat left, float right)
        {
            return (float)left.value == right;
        }

        public static bool operator !=(dtFloat left, float right)
        {
            return (float)left.value != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtFloat other)
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