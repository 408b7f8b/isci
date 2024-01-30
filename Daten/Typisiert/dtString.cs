using System;
using System.Linq;

namespace isci.Daten
{
    public class dtString : Dateneintrag
    {
        public dtString(String value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.String;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (String)reader.ReadString();
            if (tmp != (String)value)
            {
                value = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((String)this.value);
        }

        public override string WertSerialisieren()
        {
            return (String)this.value;
        }

        public override void WertAusString(System.String s)
        {
            value = s;
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<String>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            value = System.Text.Encoding.UTF8.GetString(bytes);
        }

        public override byte[] WertNachBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes((String)value);
        }

        public static bool operator ==(dtString left, dtString right)
        {
            return (String)left.value == (String)right.value;
        }

        public static bool operator !=(dtString left, dtString right)
        {
            return (String)left.value != (String)right.value;
        }

        public static bool operator ==(dtString left, String right)
        {
            return (String)left.value == right;
        }

        public static bool operator !=(dtString left, String right)
        {
            return (String)left.value != right;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is dtString other)
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