using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt16 : Dateneintrag
    {
        public new System.Int16 Wert
        {
            get
            {
                return (Int16)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtInt16(System.Int16 Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int16;
            this.Wert = Wert;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt16();
            if (tmp != (System.Int16)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Int16)Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.Int16.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.Int16>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToInt16(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtInt16 left, dtInt16 right)
        {
            return (System.Int16)left.Wert == (System.Int16)right.Wert;
        }

        public static bool operator !=(dtInt16 left, dtInt16 right)
        {
            return (System.Int16)left.Wert != (System.Int16)right.Wert;
        }

        public static bool operator ==(dtInt16 left, System.Int16 right)
        {
            return (System.Int16)left.Wert == right;
        }

        public static bool operator !=(dtInt16 left, System.Int16 right)
        {
            return (System.Int16)left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt16 other)
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