using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt8 : Dateneintrag
    {
        public new System.SByte Wert
        {
            get
            {
                return (sbyte)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtInt8(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int8;
            this.Wert = 0;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public dtInt8(System.SByte Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int8;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadSByte();
            if (tmp != (System.SByte)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.SByte)Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.SByte.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.SByte>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = (sbyte)bytes[0];
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtInt8 left, dtInt8 right)
        {
            return (System.SByte)left.Wert == (System.SByte)right.Wert;
        }

        public static bool operator !=(dtInt8 left, dtInt8 right)
        {
            return (System.SByte)left.Wert != (System.SByte)right.Wert;
        }

        public static bool operator ==(dtInt8 left, System.SByte right)
        {
            return (System.SByte)left.Wert == right;
        }

        public static bool operator !=(dtInt8 left, System.SByte right)
        {
            return (System.SByte)left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt8 other)
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