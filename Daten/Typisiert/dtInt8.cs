using System;
using System.Linq;

namespace isci.Daten
{
    public class dtInt8 : Dateneintrag
    {
        //public new System.Int32 value;

        public dtInt8(System.SByte value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Int8;
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadSByte();
            if (tmp != (System.SByte)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.SByte)value);
        }

        public override string Serialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void AusString(System.String s)
        {
            value = System.SByte.Parse(s);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.SByte>();
        }

        public static bool operator ==(dtInt8 left, dtInt8 right)
        {
            return (System.SByte)left.value == (System.SByte)right.value;
        }

        public static bool operator !=(dtInt8 left, dtInt8 right)
        {
            return (System.SByte)left.value != (System.SByte)right.value;
        }

        public static bool operator ==(dtInt8 left, System.SByte right)
        {
            return (System.SByte)left.value == right;
        }

        public static bool operator !=(dtInt8 left, System.SByte right)
        {
            return (System.SByte)left.value != right;
        }
    }
}