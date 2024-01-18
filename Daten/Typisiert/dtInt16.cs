using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt16 : Dateneintrag
    {
        //public new System.Int32 value;

        public dtInt16(System.Int16 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int16;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt16();
            if (tmp != (System.Int16)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Int16)value);
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = System.Int16.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.Int16>();
        }

        public static bool operator ==(dtInt16 left, dtInt16 right)
        {
            return (System.Int16)left.value == (System.Int16)right.value;
        }

        public static bool operator !=(dtInt16 left, dtInt16 right)
        {
            return (System.Int16)left.value != (System.Int16)right.value;
        }

        public static bool operator ==(dtInt16 left, System.Int16 right)
        {
            return (System.Int16)left.value == right;
        }

        public static bool operator !=(dtInt16 left, System.Int16 right)
        {
            return (System.Int16)left.value != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt16 other)
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