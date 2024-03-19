using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt32 : Dateneintrag
    {
        public new System.UInt32 Wert
        {
            get
            {
                return (UInt32)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtUInt32(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt32;
            this.Wert = 0;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        [Newtonsoft.Json.JsonConstructor]
        public dtUInt32(System.UInt32 Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt32;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadUInt32();
            if (tmp != (System.UInt32)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.UInt32)Wert);
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = System.UInt32.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.UInt32>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToUInt32(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtUInt32 left, dtUInt32 right)
        {
            return (System.UInt32)left.Wert == (System.UInt32)right.Wert;
        }

        public static bool operator !=(dtUInt32 left, dtUInt32 right)
        {
            return (System.UInt32)left.Wert != (System.UInt32)right.Wert;
        }

        public static bool operator ==(dtUInt32 left, System.UInt32 right)
        {
            return (System.UInt32)left.Wert == right;
        }

        public static bool operator !=(dtUInt32 left, System.UInt32 right)
        {
            return (System.UInt32)left.Wert != right;
        }

        public static dtUInt32 operator ++(dtUInt32 element)
        {
            element.Wert++;
            return element;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtUInt32 other)
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