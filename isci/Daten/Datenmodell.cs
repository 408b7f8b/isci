using System;
using System.Linq;
using isci.Allgemein;

namespace isci.Daten
{
    public class Datenmodell : Header
    {
        public ListeDateneintraege Dateneinträge;
        public VerweislisteDateneintraege Links;

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
                var de = Dateneintrag.DatafieldTyped(entry);
                dm.Dateneinträge.Add(de);
            }

            var link_entries = datamodel_.SelectToken("Links");

            try {
                dm.Links = link_entries.ToObject<VerweislisteDateneintraege>();
            } catch {

            }

            return dm;
        }

        public void AddEvaluationSet(int size, string identifikation)
        {
            for(int i = 0; i < size; ++i)
            {
                Dateneinträge.Add(new dtInt32(0, identifikation + i.ToString()));
            }
        }

        public void AddEvaluationLead(int size)
        {
            AddEvaluationSet(size, "evalLead_");
        }
    }
}