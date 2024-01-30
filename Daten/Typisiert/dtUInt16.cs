using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt16 : Dateneintrag
    {
        public new System.UInt32 value;

        public dtUInt16(System.UInt16 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt16;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadUInt16();
            if (tmp != value)
            {
                value = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write(value);
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = System.UInt16.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.UInt16>();
        }

        public static bool operator ==(dtUInt16 left, dtUInt16 right)
        {
            return left.value == right.value;
        }

        public static bool operator !=(dtUInt16 left, dtUInt16 right)
        {
            return left.value != right.value;
        }

        public static bool operator ==(dtUInt16 left, System.UInt16 right)
        {
            return left.value == right;
        }

        public static bool operator !=(dtUInt16 left, System.UInt16 right)
        {
            return left.value != right;
        }

        public static dtUInt16 operator ++(dtUInt16 element)
        {
            element.value++;
            return element;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtUInt16 other)
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