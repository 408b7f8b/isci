﻿using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Reflection;

namespace isci.Daten
{
    public class dtObjekt : Dateneintrag
    {
#pragma warning disable CS0169
        private new EinheitenKodierung Einheit;

#pragma warning restore CS0169
        public List<string> Elemente;
        public List<Dateneintrag> ElementeLaufzeit;

        public dtObjekt(String Identifikation) : base(Identifikation)
        {
            this.Identifikation = Identifikation;
            this.type = Datentypen.Objekt;

            if (ElementeLaufzeit != null)
            {
                foreach (var Element in ElementeLaufzeit)
                {
                    if (!Elemente.Contains(Element.Identifikation)) Elemente.Add(Element.Identifikation);
                }
            } else {

                ElementeLaufzeit = new List<Dateneintrag>();
            }

            if (Elemente == null) Elemente = new List<string>();

            var typ = this.GetType();
            var felder = typ.GetRuntimeFields();

            foreach (var feld in felder)
            {
                var typ2 = feld.FieldType;
                var typ3 = typ2.BaseType;
                if (typ3 == typeof(Dateneintrag))
                {
                    var instanz = feld.GetValue(this);
                    if (instanz == null)
                    {
                        var instanz_typisiert = Activator.CreateInstance(typ2, this.Identifikation + "." + feld.Name, "");
                        feld.SetValue(this, instanz_typisiert);
                        instanz = instanz_typisiert;
                    }

                    ((Dateneintrag)instanz).Identifikation = this.Identifikation + "." + feld.Name;
                    ((Dateneintrag)instanz).parentEintrag = this.Identifikation;
                    if (!ElementeLaufzeit.Contains((Dateneintrag)instanz)) ElementeLaufzeit.Add((Dateneintrag)instanz);

                    if (!Elemente.Contains(this.Identifikation + "." + feld.Name)) Elemente.Add(this.Identifikation + "." + feld.Name);
                }
            }
        }

        public new void WertAusSpeicherLesen()
        {
            foreach (var element in ElementeLaufzeit)
            {
                element.WertAusSpeicherLesen();
            }
        }

        /* public new void WertInSpeicherSchreiben()
        {

        } */

        public override void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {
            
        }

        public override void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            
        }

        public override string WertSerialisieren()
        {
            //var r = "{\n";
            var r = "";
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
                    //r += ",\n";
                    r += ",";
                } //else {
                    //r += "\n";
                //}
            }
            r += "}";

            return r;
        }

        public override void WertAusString(System.String s)
        {
            var tmp = Newtonsoft.Json.Linq.JToken.Parse(s);
            WertAusJTokenSpezifisch(tmp);
        }

        public override void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token)
        {
            foreach (var element in ElementeLaufzeit)
            {
                try {
                    element.WertAusJTokenSpezifisch(token.SelectToken(element.Identifikation));
                } catch {

                }
            }
        }

        public override void WertAusBytes(byte[] bytes)
        {
            //Wert = BitConverter.ToInt16(bytes, 0);
        }

        public override byte[] WertNachBytes()
        {
            //return BitConverter.GetBytes(Wert);
            return null;
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

        public override bool Equals(object obj)
        {
            if (obj is dtObjekt other)
            {
                return Wert == other.Wert; //implementierung nicht korrekt!
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Wert.GetHashCode();
        }
    }
}