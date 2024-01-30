using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt32 : Dateneintrag
    {
        public new System.Int32 value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
                this.aenderungIntern = true;
            }
        }

        public dtInt32(System.Int32 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int32;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt32();
            if (tmp != value)
            {
                value = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write(value);
        }

        public override string WertSerialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            value = System.Int32.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.Int32>();
        }

        public static bool operator ==(dtInt32 left, dtInt32 right)
        {
            return left.value == right.value;
        }

        public static bool operator !=(dtInt32 left, dtInt32 right)
        {
            return left.value != right.value;
        }

        public static bool operator ==(dtInt32 left, System.Int32 right)
        {
            return left.value == right;
        }

        public static bool operator !=(dtInt32 left, System.Int32 right)
        {
            return left.value != right;
        }

        public override bool Equals(object obj)
        {
            if (obj is dtInt32 other)
            {
                return value == other.value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}