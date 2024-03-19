using System;
using System.ComponentModel;
using System.Linq;

namespace isci.Daten
{
    public class dtDouble : Dateneintrag
    {
        public new System.Double Wert
        {
            get
            {
                return (double)Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        public dtDouble(String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Double;
            this.Wert = 0.0;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        [Newtonsoft.Json.JsonConstructor]
        public dtDouble(Double Wert, String Identifikation, String path = "") : base(Identifikation)
        {
            this.type = Datentypen.Double;
            this.Wert = Wert;
            this.aenderungIntern = false;
            if (path != "") this.path = path;
        }

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = (Double)reader.ReadDouble();
            if (tmp != (Double)Wert)
            {
                Wert = tmp;
                aenderungExtern = true;
            }
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((double)((Double)Wert));
        }

        public override string WertSerialisieren()
        {
            var s = Wert.ToString();
            return s;
        }

        public override void WertAusString(System.String s)
        {
            Wert = Double.Parse(s);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            Wert = token.ToObject<Double>();
        }

        public override void WertAusBytes(byte[] bytes)
        {
            Wert = BitConverter.ToDouble(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            return BitConverter.GetBytes(Wert);
        }

        public static bool operator ==(dtDouble left, dtDouble right)
        {
            return (Double)left.Wert == (Double)right.Wert;
        }

        public static bool operator !=(dtDouble left, dtDouble right)
        {
            return (Double)left.Wert != (Double)right.Wert;
        }

        public static bool operator ==(dtDouble left, Double right)
        {
            return (Double)left.Wert == right;
        }

        public static bool operator !=(dtDouble left, Double right)
        {
            return (Double)left.Wert != right;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is dtDouble other)
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