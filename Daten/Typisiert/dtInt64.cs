using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt64 : Dateneintrag
    {
        public new System.Int64 Wert
        {
            get
            {
                return (Int64)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtInt64(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int64;
            this.Wert = 0;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public dtInt64(System.Int64 Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int64;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt64();
            if (tmp != Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write(Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.Int64.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.Int64>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToInt64(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtInt64 left, dtInt64 right)
        {
            return left.Wert == right.Wert;
        }

        public static bool operator !=(dtInt64 left, dtInt64 right)
        {
            return left.Wert != right.Wert;
        }

        public static bool operator ==(dtInt64 left, System.Int64 right)
        {
            return left.Wert == right;
        }

        public static bool operator !=(dtInt64 left, System.Int64 right)
        {
            return left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt64 other)
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