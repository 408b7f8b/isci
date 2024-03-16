using System;
using System.Linq;

namespace isci.Daten
{
    public class dtString : Dateneintrag
    {
        public new System.String Wert
        {
            get
            {
                return (string)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtString(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.String;
            this.Wert = "";
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public dtString(String Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.String;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (String)reader.ReadString();
            if (tmp != (String)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((String)this.Wert);
        }

        public override string WertSerialisieren()
        {
            return (String)this.Wert;
        }

        public override void WertAusString(System.String s)
        {
            Wert = s;
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<String>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = System.Text.Encoding.UTF8.GetString(bytes);
        }

        public override byte[] WertNachBytes()
        {
            return System.Text.Encoding.UTF8.GetBytes((String)Wert);
        }

        public static bool operator ==(dtString left, dtString right)
        {
            return (String)left.Wert == (String)right.Wert;
        }

        public static bool operator !=(dtString left, dtString right)
        {
            return (String)left.Wert != (String)right.Wert;
        }

        public static bool operator ==(dtString left, String right)
        {
            return (String)left.Wert == right;
        }

        public static bool operator !=(dtString left, String right)
        {
            return (String)left.Wert != right;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is dtString other)
            {
                return Wert == other.Wert;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Wert.GetHashCode();
        }
    }
}