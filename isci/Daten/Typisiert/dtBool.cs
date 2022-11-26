using System;
using System.Linq;

namespace isci.Daten
{
    public class dtBool : Dateneintrag
    {
        public dtBool(System.Boolean value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadBoolean();
            if (tmp != (System.Boolean)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Boolean)value);
        }

        public override string Serialisieren()
        {
            var s = value.ToString();
            return s;
        }

        public override void AusString(System.String s)
        {
            value = System.Boolean.Parse(s);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            value = token.ToObject<System.Boolean>();
        }

        public static bool operator ==(dtBool left, dtBool right)
        {
            return (System.Boolean)left.value == (System.Boolean)right.value;
        }

        public static bool operator !=(dtBool left, dtBool right)
        {
            return (System.Boolean)left.value != (System.Boolean)right.value;
        }

        public static bool operator ==(dtBool left, System.Boolean right)
        {
            return (System.Boolean)left.value == right;
        }

        public static bool operator !=(dtBool left, System.Boolean right)
        {
            return (System.Boolean)left.value != right;
        }
    }
}