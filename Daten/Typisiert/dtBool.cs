using System;
using System.Linq;

namespace isci.Daten
{
    public class dtBool : Dateneintrag
    {
        public dtBool(System.Boolean value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Bool;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadBoolean();
            if (tmp != (System.Boolean)value)
            {
                value = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Boolean)value);
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = System.Boolean.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.Boolean>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            value = BitConverter.ToBoolean(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes((System.Boolean)value);
        }

        public static bool operator ==(dtBool left, dtBool right)
        {
            return (System.Boolean)left.value == (System.Boolean)right.value;
        }

        public static bool operator !=(dtBool left, dtBool right)
        {
            return (System.Boolean)left.value != (System.Boolean)right.value;
        }

        public static bool operator ==(dtBool left, System.Boolean right)
        {
            return (System.Boolean)left.value == right;
        }

        public static bool operator !=(dtBool left, System.Boolean right)
        {
            return (System.Boolean)left.value != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtBool other)
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