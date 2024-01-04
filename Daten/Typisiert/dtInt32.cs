using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt32 : Dateneintrag
    {
        //public new System.Int32 value;

        public dtInt32(System.Int32 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int32;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt32();
            if (tmp != (System.Int32)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Int32)value);
        }

        public override string Serialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void AusString(System.String s)
        {
            value = System.Int32.Parse(s);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.Int32>();
        }

        public static bool operator ==(dtInt32 left, dtInt32 right)
        {
            return (System.Int32)left.value == (System.Int32)right.value;
        }

        public static bool operator !=(dtInt32 left, dtInt32 right)
        {
            return (System.Int32)left.value != (System.Int32)right.value;
        }

        public static bool operator ==(dtInt32 left, System.Int32 right)
        {
            return (System.Int32)left.value == right;
        }

        public static bool operator !=(dtInt32 left, System.Int32 right)
        {
            return (System.Int32)left.value != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt32 other)
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