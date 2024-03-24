using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using isci.Allgemein;

namespace isci.Daten
{
    public class Datenmodell : Header
    {
        public string Stand;
        public string Ablauf;
        public ListeDateneintraege Dateneinträge;
        public VerweislisteDateneintraege Links;

        public Datenmodell(isci.Allgemein.Parameter parameter, ListeDateneintraege Dateneinträge = null, string Stand = null, string Ablauf = null) : base(parameter.Identifikation)
        {
            if (this.Dateneinträge == null)
            {
                this.Dateneinträge = (Dateneinträge == null ? new ListeDateneintraege() : Dateneinträge);
            }
            
            if (this.Links == null) this.Links = new VerweislisteDateneintraege();
            if (Stand == null)
            {
                this.Stand = DateTime.Now.ToString("O");
            } else {
                this.Stand = Stand;
            }
        }

        public Datenmodell(string Identifikation, ListeDateneintraege Dateneinträge = null, string Stand = null, string Ablauf = null) : base(Identifikation)
        {
            if (this.Dateneinträge == null)
            {
                this.Dateneinträge = (Dateneinträge == null ? new ListeDateneintraege() : Dateneinträge);
            }
            
            if (this.Links == null) this.Links = new VerweislisteDateneintraege();
            if (Stand == null)
            {
                this.Stand = DateTime.Now.ToString("O");
            } else {
                this.Stand = Stand;
            }
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
            var pfad = parameter.OrdnerDatenmodelle + "/" + Identifikation + ".json";
            if (System.IO.File.Exists(pfad))
            {
                var modell_ = System.IO.File.ReadAllText(pfad);
                var fremdesJObject = Newtonsoft.Json.Linq.JObject.Parse(modell_);
                string fremderStand = fremdesJObject["Stand"].ToObject<String>();

                var eigenesJObject = Newtonsoft.Json.Linq.JObject.FromObject(this);

                fremdesJObject.Remove("Stand");
                eigenesJObject.Remove("Stand");
                fremdesJObject.Remove("Identifikation");
                eigenesJObject.Remove("Identifikation");

                if (Newtonsoft.Json.Linq.JToken.DeepEquals(fremdesJObject, eigenesJObject))
                {
                    if (fremderStand != null && fremderStand != "")
                    {
                        this.Stand = fremderStand;
                    }
                    return;
                } else {
                    if (Stand == null || Stand == "")
                    {
                        if (fremderStand != null && fremderStand != "")
                        {
                            uint version;
                            if (UInt32.TryParse(fremderStand, out version))
                            {
                                this.Stand = (++version).ToString();
                            } else {
                                this.Stand = DateTime.Now.ToString("O");
                            }
                        }
                    }
                }
            }

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

        public bool AbfrageNeuerAlsDatenmodell(Datenmodell dm)
        {
            return AbfrageDatenmodell1NeuerAlsDatenmodell2(this, dm);
        }

        public static bool AbfrageDatenmodell1NeuerAlsDatenmodell2(Datenmodell dm1, Datenmodell dm2)
        {
            if (dm1.Stand == "" || dm1.Stand == null)
            {
                throw new Exception("Stand des Datenmodells 1 nicht determinierbar.");
            }

            if (dm2.Stand == "" || dm2.Stand == null)
            {
                throw new Exception("Stand des Datenmodells 2 nicht determinierbar.");
            }

            UInt32 ui1, ui2;
            if (UInt32.TryParse(dm1.Stand, out ui1))
            {
                if (UInt32.TryParse(dm2.Stand, out ui2))
                {
                    return ui1 > ui2; //beide sind versioniert
                } else {
                    return true; //nur dm1 ist versioniert
                }
            } else {
                if (UInt32.TryParse(dm2.Stand, out ui2))
                {
                    return false; //nur dm2 ist versioniert
                } else {
                    DateTime dt1, dt2;
                    if (!DateTime.TryParse(dm1.Stand, out dt1))
                    {
                        throw new Exception("Stand des Datenmodells 1 nicht determinierbar.");
                    }

                    if (!DateTime.TryParse(dm2.Stand, out dt2))
                    {
                        throw new Exception("Stand des Datenmodells 2 nicht determinierbar.");
                    }

                    return dt1 > dt2;
                }
            }
        }

        public void Ueberlagern(Datenmodell ueberlagerungsmodell)
        {
            if (ueberlagerungsmodell.Identifikation != null && ueberlagerungsmodell.Identifikation != "") this.Identifikation = ueberlagerungsmodell.Identifikation;
            if (ueberlagerungsmodell.Name != null && ueberlagerungsmodell.Name != "") this.Name = ueberlagerungsmodell.Name;
            if (ueberlagerungsmodell.Beschreibung != null && ueberlagerungsmodell.Beschreibung != "") this.Beschreibung = ueberlagerungsmodell.Beschreibung;

            foreach (var eintrag in ueberlagerungsmodell.Dateneinträge)
            {
                if (Dateneinträge.IdentifikationBereitsEnthalten(eintrag.Identifikation))
                {
                    if (Dateneinträge[eintrag.Identifikation].Einheit == EinheitenKodierung.keine && eintrag.Einheit != EinheitenKodierung.keine) Dateneinträge[eintrag.Identifikation].Einheit = eintrag.Einheit;
                    if (eintrag.Beschreibung != null && eintrag.Beschreibung != "") Dateneinträge[eintrag.Identifikation].Beschreibung = eintrag.Beschreibung;
                    if (eintrag.parentEintrag != null && eintrag.parentEintrag != "") Dateneinträge[eintrag.Identifikation].parentEintrag = eintrag.parentEintrag;
                }
            }
        }

        public void Ueberlagern(string pfad)
        {
            try {
                var dm = AusDatei(pfad);
                Ueberlagern(dm);
            } catch (Exception e)
            {
                Logger.Fehler("Ausnahme beim Überlagern des Datenmodells " + Identifikation + " mit dem Datenmodell " + pfad + ": " + e.Message);
            }
        }
    }
}