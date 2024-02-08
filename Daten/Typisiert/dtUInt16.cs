using System;
using System.Linq;

namespace isci.Daten
{
    public class dtUInt16 : Dateneintrag
    {
        public new System.UInt16 Wert
        {
            get
            {
                return (UInt16)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtUInt16(System.UInt16 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.UInt16;
            this.Wert = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadUInt16();
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
            Wert = System.UInt16.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<System.UInt16>();
        }

        public static bool operator ==(dtUInt16 left, dtUInt16 right)
        {
            return left.Wert == right.Wert;
        }

        public static bool operator !=(dtUInt16 left, dtUInt16 right)
        {
            return left.Wert != right.Wert;
        }

        public static bool operator ==(dtUInt16 left, System.UInt16 right)
        {
            return left.Wert == right;
        }

        public static bool operator !=(dtUInt16 left, System.UInt16 right)
        {
            return left.Wert != right;
        }

        public static dtUInt16 operator ++(dtUInt16 element)
        {
            element.Wert++;
            return element;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtUInt16 other)
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