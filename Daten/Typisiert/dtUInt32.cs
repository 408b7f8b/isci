using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt32 : Dateneintrag
    {
        //public new System.UInt32 value;

        public dtUInt32(System.UInt32 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt32;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadUInt32();
            if (tmp != (System.UInt32)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.UInt32)value);
        }

        public override string Serialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void AusString(System.String s)
        {
            value = System.UInt32.Parse(s);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.UInt32>();
        }

        public static bool operator ==(dtUInt32 left, dtUInt32 right)
        {
            return (System.UInt32)left.value == (System.UInt32)right.value;
        }

        public static bool operator !=(dtUInt32 left, dtUInt32 right)
        {
            return (System.UInt32)left.value != (System.UInt32)right.value;
        }

        public static bool operator ==(dtUInt32 left, System.UInt32 right)
        {
            return (System.UInt32)left.value == right;
        }

        public static bool operator !=(dtUInt32 left, System.UInt32 right)
        {
            return (System.UInt32)left.value != right;
        }
    }
}