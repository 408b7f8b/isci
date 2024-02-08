using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt8 : Dateneintrag
    {
        public new System.Byte Wert
        {
            get
            {
                return (byte)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtUInt8(System.Byte Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt8;
            this.Wert = Wert;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadByte();
            if (tmp != (System.Byte)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Byte)Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.Byte.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.Byte>();
        }

        public static bool operator ==(dtUInt8 left, dtUInt8 right)
        {
            return (System.Byte)left.Wert == (System.Byte)right.Wert;
        }

        public static bool operator !=(dtUInt8 left, dtUInt8 right)
        {
            return (System.Byte)left.Wert != (System.Byte)right.Wert;
        }

        public static bool operator ==(dtUInt8 left, System.Byte right)
        {
            return (System.Byte)left.Wert == right;
        }

        public static bool operator !=(dtUInt8 left, System.Byte right)
        {
            return (System.Byte)left.Wert != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtUInt8 other)
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