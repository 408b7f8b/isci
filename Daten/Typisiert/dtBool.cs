using System;
using System.Linq;

namespace isci.Daten
{
    public class dtBool : Dateneintrag
    {
        public new System.Boolean Wert
        {
            get
            {
                return (System.Boolean)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtBool(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Bool;
            this.Wert = false;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public dtBool(System.Boolean Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Bool;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadBoolean();
            if (tmp != (System.Boolean)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Boolean)Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.Boolean.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.Boolean>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToBoolean(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes((System.Boolean)Wert);
        }

        public static bool operator ==(dtBool left, dtBool right)
        {
            return (System.Boolean)left.Wert == (System.Boolean)right.Wert;
        }

        public static bool operator !=(dtBool left, dtBool right)
        {
            return (System.Boolean)left.Wert != (System.Boolean)right.Wert;
        }

        public static bool operator ==(dtBool left, System.Boolean right)
        {
            return (System.Boolean)left.Wert == right;
        }

        public static bool operator !=(dtBool left, System.Boolean right)
        {
            return (System.Boolean)left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtBool other)
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