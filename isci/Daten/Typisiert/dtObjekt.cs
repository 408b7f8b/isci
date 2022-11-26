using System;
using System.Collections.Generic;

namespace isci.Daten
{
    public class dtObjekt : Dateneintrag
    {
        public List<string> Elemente;
        public List<Dateneintrag> ElementeLaufzeit;

        public dtObjekt(String Identifikation) : base(Identifikation)
        {

        }

        public new void Lesen()
        {
            foreach (var element in ElementeLaufzeit)
            {
                element.Lesen();
            }
        }

        public override void LesenSpezifisch(System.IO.BinaryReader reader)
        {
            
        }

        public override void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            
        }

        public override string Serialisieren()
        {
            var r = "{\n";
            foreach (var element in ElementeLaufzeit)
            {
                if (element.type == Datentypen.String)
                {
                    r += "\"" + element.Identifikation + "\":\"" + element.ToString() + "\"";
                } else {
                    r += "\"" + element.Identifikation + "\":" + element.ToString();
                }                

                if (ElementeLaufzeit.IndexOf(element) != ElementeLaufzeit.Count-1)
                {
                    r += ",\n";
                } else {
                    r += "\n";
                }
            }
            r += "}";

            return r;
        }

        public override void AusString(System.String s)
        {
            var tmp = Newtonsoft.Json.Linq.JToken.Parse(s);
            AusJTokenSpezifisch(tmp);
        }

        public override void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            foreach (var element in ElementeLaufzeit)
            {
                try {
                    element.AusJTokenSpezifisch(token.SelectToken(element.Identifikation));
                } catch {

                }
            }
        }

        public static bool operator ==(dtObjekt left, dtObjekt right)
        {
            if (left.Elemente.Count != right.Elemente.Count) return false;
            foreach (var element in left.Elemente)
            {
                if (!right.Elemente.Contains(element)) return false;
            }
            return true;
        }

        public static bool operator !=(dtObjekt left, dtObjekt right)
        {
            return !(left == right);
        }
    }
}