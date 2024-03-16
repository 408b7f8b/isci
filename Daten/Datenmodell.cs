using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using isci.Allgemein;

namespace isci.Daten
{
    public class Datenmodell : Header
    {
        public string Stand;
        public ListeDateneintraege Dateneinträge;
        public VerweislisteDateneintraege Links;

        public Datenmodell(isci.Allgemein.Parameter parameter, ListeDateneintraege Dateneinträge = null) : base(parameter.Identifikation)
        {
            if (this.Dateneinträge == null)
            {
                this.Dateneinträge = (Dateneinträge == null ? new ListeDateneintraege() : Dateneinträge);
            }
            
            if (this.Links == null) this.Links = new VerweislisteDateneintraege();
        }

        public Datenmodell(string Identifikation, ListeDateneintraege Dateneinträge = null) : base(Identifikation)
        {
            if (this.Dateneinträge == null)
            {
                this.Dateneinträge = (Dateneinträge == null ? new ListeDateneintraege() : Dateneinträge);
            }
            
            if (this.Links == null) this.Links = new VerweislisteDateneintraege();
        }

        public static Datenmodell AusDatei(string path)
        {
            var file = System.IO.File.ReadAllText(path);
            var datamodel_ = Newtonsoft.Json.Linq.JObject.Parse(file);

            var datamodel = AusJObject(datamodel_);

            return datamodel;
        }

        public void Speichern(Parameter parameter)
        {
            Speichern(parameter.OrdnerDatenmodelle + "/" + Identifikation + ".json");
        }

        public override string ToString()
        {
            var settings = new Newtonsoft.Json.JsonSerializerSettings();
            settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            var ret = Newtonsoft.Json.JsonConvert.SerializeObject(this, settings);
            return ret;
        }

        public void ToFile(string path)
        {
            var file = this.ToString();
            System.IO.File.WriteAllText(path, file);
        }

        public static Datenmodell AusJObject(Newtonsoft.Json.Linq.JObject datamodel_)
        {
            var dm = new Datenmodell(datamodel_.SelectToken("Identifikation").ToString());

            var entries = datamodel_.SelectToken("Dateneinträge");

            foreach (Newtonsoft.Json.Linq.JObject entry in entries)
            {
                var de = Dateneintrag.GibDateneintragTypisiert(entry);
                dm.Dateneinträge.Add(de);
            }

            var link_entries = datamodel_.SelectToken("Links");

            try {
                dm.Links = link_entries.ToObject<VerweislisteDateneintraege>();
            } catch {

            }

            var stand = datamodel_.SelectToken("Stand");

            try {
                dm.Stand = stand.ToObject<string>();
            } catch {

            }

            return dm;
        }

        public Dateneintrag this[string key]
        {
            get
            {
                Dateneintrag ret = null;
                ret = this.Dateneinträge.First(a => a.Identifikation == key);
                return ret;
            }
        }

        public void Add(Dateneintrag dateneintrag)
        {
            dateneintrag.korrigiereIdentifikationFallsNotw(this.Identifikation);

            this.Dateneinträge.Add(dateneintrag);

            if (dateneintrag.type == Datentypen.Objekt)
            {
                var dt_tmp = (dtObjekt)dateneintrag;
                foreach (var Element in dt_tmp.ElementeLaufzeit)
                {
                    dt_tmp.Elemente.Remove(Element.Identifikation);
                    Element.parentEintrag = dt_tmp.Identifikation;
                    this.Add(Element);
                    dt_tmp.Elemente.Add(Element.Identifikation);
                }
            }
        }

        public List<string> Identifikatoren()
        {
            return this.Dateneinträge.Identifikatoren();
        }
    }
}