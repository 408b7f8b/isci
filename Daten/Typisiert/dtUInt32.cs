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

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadUInt32();
            if (tmp != (System.UInt32)value)
            {
                value = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.UInt32)value);
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = System.UInt32.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
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

        public override bool Equals(object obj)
        {
            if (obj is dtUInt32 other)
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