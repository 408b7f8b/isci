using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt32 : Dateneintrag
    {
        public new System.Int32 Wert
        {
            get
            {
                return (Int32)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtInt32(System.Int32 Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int32;
            this.Wert = Wert;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt32();
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
            Wert = System.Int32.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.Int32>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToInt32(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtInt32 left, dtInt32 right)
        {
            return left.Wert == right.Wert;
        }

        public static bool operator !=(dtInt32 left, dtInt32 right)
        {
            return left.Wert != right.Wert;
        }

        public static bool operator ==(dtInt32 left, System.Int32 right)
        {
            return left.Wert == right;
        }

        public static bool operator !=(dtInt32 left, System.Int32 right)
        {
            return left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt32 other)
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