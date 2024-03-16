using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt64 : Dateneintrag
    {
        public new System.UInt64 Wert
        {
            get
            {
                return (UInt64)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtUInt64(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt64;
            this.Wert = 0;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public dtUInt64(System.UInt64 Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt64;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadUInt64();
            if (tmp != (System.UInt64)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.UInt64)Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.UInt64.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.UInt64>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToUInt64(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtUInt64 left, dtUInt64 right)
        {
            return (System.UInt64)left.Wert == (System.UInt64)right.Wert;
        }

        public static bool operator !=(dtUInt64 left, dtUInt64 right)
        {
            return (System.UInt64)left.Wert != (System.UInt64)right.Wert;
        }

        public static bool operator ==(dtUInt64 left, System.UInt64 right)
        {
            return (System.UInt64)left.Wert == right;
        }

        public static bool operator !=(dtUInt64 left, System.UInt64 right)
        {
            return (System.UInt64)left.Wert != right;
        }

        public static dtUInt64 operator ++(dtUInt64 element)
        {
            element.Wert++;
            return element;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtUInt64 other)
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