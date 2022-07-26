using System;
using System.Linq;

namespace library
{
    public static class Helfer
    {
        private static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> Cache;

        public static System.Collections.Generic.List<string> ChangedFiles(string path)
        {
            var files_ = System.IO.Directory.GetFiles(path);
            var files = new System.Collections.Generic.List<string>(files_);

            if (Cache.ContainsKey(path))
            {
                var firstNotSecond = files.Except(Cache[path]).ToList();
                var secondNotFirst = Cache[path].Except(files).ToList();

                if (firstNotSecond.Count > 0) {
                    Cache[path].AddRange(firstNotSecond);
                }

                foreach (var s in secondNotFirst)
                {
                    Cache[path].Remove(s);
                }
            } else {
                Cache.Add(path, files);
            }
            return Cache[path];
        }

        public static void OrdnerPruefenErstellen(string pfad)
        {
            if (!System.IO.Directory.Exists(pfad)) System.IO.Directory.CreateDirectory(pfad);
        }
    }

    public class Modell
    {
        public string Identifikation;
        public string Name;
        public string Beschreibung;
        public string Typ;

        public Modell()
        {
            this.Typ = this.GetType().FullName;
        }

        public void Speichern(string pfad)
        {
            try {
                var ser = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                Helfer.OrdnerPruefenErstellen(System.IO.Path.GetDirectoryName(pfad));
                System.IO.File.WriteAllText(pfad, ser);
            } catch {

            }            
        }
    }

    public class RessourceList : System.Collections.Generic.List<string>
    {

    }

    public class Zustandsbereich
    {
        public int Arbeitszustand;
        public int Nachfolgezustand;
    }

    public class Konfiguration
    {
        public string Ressource;
        public string Identifikation;
        public string OrdnerAnwendung;
        public string OrdnerDatenstruktur;
        public string Anwendung;
        public string OrdnerDatenmodelle;
        public string OrdnerEreignismodelle;
        public string OrdnerFunktionsmodelle;
        public string OrdnerBeschreibungen;
        public Zustandsbereich[] Zustandsbereiche;

        public Konfiguration()
        {

        }

        public Konfiguration(string datei)
        {
            Ressource = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_RESSOURCE");

            if (!System.IO.File.Exists(datei))
            {
                return;
            }
            else
            {
                try
                {
                    var t = Newtonsoft.Json.Linq.JToken.Parse(System.IO.File.ReadAllText(datei));
                    Console.WriteLine("Konfigdatei: " + datei);

                    var felder = GetType().GetFields();

                    foreach (var f in felder)
                    {
                        try
                        {
                            var feld = GetType().GetField(f.Name);
                            var feldtyp = feld.FieldType;

                            if (feldtyp == typeof(string))
                            {
                                var o = t.SelectToken(f.Name).ToObject<string>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(int))
                            {
                                var o = t.SelectToken(f.Name).ToObject<int>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(uint))
                            {
                                var o = t.SelectToken(f.Name).ToObject<uint>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(bool))
                            {
                                var o = t.SelectToken(f.Name).ToObject<bool>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(double))
                            {
                                var o = t.SelectToken(f.Name).ToObject<double>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(Int32[]))
                            {
                                var o = t.SelectToken(f.Name).ToObject<Int32[]>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(string[]))
                            {
                                var o = t.SelectToken(f.Name).ToObject<string[]>();
                                feld.SetValue(this, o);
                            }
                            else if (feldtyp == typeof(Zustandsbereich[]))
                            {
                                var o = t.SelectToken(f.Name).ToObject<Zustandsbereich[]>();
                                feld.SetValue(this, o);
                            }

                            try {
                                Console.WriteLine("Konfigparam " + feld.Name + ": " + feld.GetValue(this).ToString());
                            } catch { }
                        }
                        catch { }
                    }
                } catch { }
            }

            OrdnerAnwendung = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung;
            OrdnerDatenstruktur = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_DATENSTRUKTUREN") + "/" + Anwendung;
            OrdnerDatenmodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Datenmodelle";
            OrdnerEreignismodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Ereignismodelle";
            OrdnerFunktionsmodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Funktionsmodelle";
            OrdnerBeschreibungen = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Beschreibungen";

            Helfer.OrdnerPruefenErstellen(OrdnerAnwendung);
            Helfer.OrdnerPruefenErstellen(OrdnerDatenstruktur);
            Helfer.OrdnerPruefenErstellen(OrdnerDatenmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerEreignismodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerFunktionsmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerBeschreibungen);
        }
    }

    public class FieldList : System.Collections.Generic.List<Datafield>
    {
        public new void Add(Datafield item)
        {
            if (this.Where(entry => entry.Identifikation == item.Identifikation).Count() > 0) return;
            /*foreach (var entry in this)
            {
                if (entry.Identifikation == item.Identifikation)
                {
                    return;
                }
            }*/
            base.Add(item);
        }

        public void AddRange(FieldList items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }
    }

    public class LinkDictionary : System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>>
    {
        public void Add(LinkDictionary item)
        {
            foreach (var entry in item)
            {
                this.Add(entry.Key, entry.Value);
            }        
        }

        public new void Add(string item1, System.Collections.Generic.List<string> item2)
        {
            if (this.ContainsKey(item1))
            {
                foreach (var entry in item2)
                {
                    if (!this[item1].Contains(entry))
                    {
                        this[item1].Add(entry);
                    }
                }
            } else {
                base.Add(item1, item2);
            }            
        }

        public void Add(string item1, string item2)
        {
            if (this.ContainsKey(item1))
            {
                if (!this[item1].Contains(item2))
                {
                    this[item1].Add(item2);
                }
            } else {
                base.Add(item1, new System.Collections.Generic.List<string>(){item2});
            }
        }
    }

    public class LinkDictionaryActive : System.Collections.Generic.Dictionary<Datafield, FieldList>
    {
        public void Add(LinkDictionaryActive item)
        {
            foreach (var entry in item)
            {
                this.Add(entry.Key, entry.Value);
            }        
        }

        public new void Add(Datafield item1, FieldList item2)
        {
            if (this.ContainsKey(item1))
            {
                foreach (var entry in item2)
                {
                    if (!this[item1].Contains(entry))
                    {
                        this[item1].Add(entry);
                    }
                }
            } else {
                base.Add(item1, item2);
            }            
        }

        public void Add(Datafield item1, Datafield item2)
        {
            if (this.ContainsKey(item1))
            {
                if (!this[item1].Contains(item2))
                {
                    this[item1].Add(item2);
                }
            } else {
                base.Add(item1, new FieldList(){item2});
            }
        }
    }

    public class Ereignis : Modell
    {
        public string Ausloeser;
        public string[] Datenfelder;
    }

    public class Funktion : Modell
    {
        public string[] Datenfelder;
    }

    public class Datamodel
    {
        public string Identifikation;
        public FieldList Datafields;
        public LinkDictionary Links;

        public Datamodel()
        {
            Datafields = new FieldList();
        }
        public Datamodel(string Identifikation)
        {
            this.Identifikation = Identifikation;
            Datafields = new FieldList();
            Links = new LinkDictionary();
        }

        public static Datamodel FromFile(string path)
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

        public static Datamodel AusJObject(Newtonsoft.Json.Linq.JObject datamodel_)
        {
            var dm = new Datamodel();
            dm.Identifikation = datamodel_.SelectToken("Identifikation").ToString();
            dm.Datafields = new FieldList();
            dm.Links = new LinkDictionary();

            var entries = datamodel_.SelectToken("Datafields");

            foreach (Newtonsoft.Json.Linq.JObject entry in entries)
            {
                var de = Datafield.DatafieldTyped(entry);
                dm.Datafields.Add(de);
            }

            var link_entries = datamodel_.SelectToken("Links");

            try {
                dm.Links = link_entries.ToObject<LinkDictionary>();
            } catch {

            }

            return dm;
        }

        public void AddEvaluationSet(int size, string identifikation)
        {
            for(int i = 0; i < size; ++i)
            {
                Datafields.Add(new var_int(0, identifikation + i.ToString()));
            }
        }

        public void AddEvaluationLead(int size)
        {
            AddEvaluationSet(size, "evalLead_");
        }
    }

    public class Datastructure
    {
        public System.Collections.Generic.List<string> Datamodels;
        public System.Collections.Generic.Dictionary<string, Datafield> Datafields;
        private LinkDictionary Links;
        public LinkDictionaryActive LinksActive;
        public string path;

        public Datastructure(string path)
        {
            Datamodels = new System.Collections.Generic.List<string>();
            Datafields = new System.Collections.Generic.Dictionary<string, Datafield>();
            Links = new LinkDictionary();
            LinksActive = new LinkDictionaryActive();
            this.path = path;
        }

        public void AddDatamodel(Datamodel datamodel)
        {
            try
            {
                foreach (var entry in datamodel.Datafields)
                {
                    string ident = "ns=" + datamodel.Identifikation + ";s=" + entry.Identifikation;
                    entry.Identifikation = ident;
                    entry.path = path + "/" + entry.Identifikation;
                    Datafields.Add(ident, entry);
                }
                foreach (var entry in datamodel.Links)
                {
                    var Links_ = new LinkDictionary();
                    var key = entry.Key;
                    var value = new System.Collections.Generic.List<string>();
                    if (!key.StartsWith("ns="))
                    {
                        key = "ns=" + datamodel.Identifikation + ";s=" + key;
                    }
                    foreach (var subentry in entry.Value)
                    {
                        if (subentry.StartsWith("ns=")) value.Add(subentry);
                        else value.Add("ns=" + datamodel.Identifikation + ";s=" + subentry);
                    }
                    Links.Add(key, value);
                }
                Datamodels.Add(datamodel.Identifikation);
            } catch (Exception)
            {
                
            }            
        }

        public void AddDatamodelFromFile(string path)
        {
            var dm_ = Datamodel.FromFile(path);
            AddDatamodel(dm_);
        }

        public void AddDataModelsFromDirectory(string path, string excludeown = "")
        {
            var dms_ = System.IO.Directory.GetFiles(path);
            foreach (var dm_ in dms_)
            {
                if (excludeown != "")
                {
                    if (dm_.Contains(excludeown)) continue;
                }                
                AddDatamodelFromFile(dm_);
            }
        }

        public void AddDataModelsFromDirectories(string[] paths, string excludeown = "")
        {
            foreach (var path in paths)
            {
                AddDataModelsFromDirectory(path, excludeown);
            }
        }

        public void GenerateLinks()
        {
            foreach (var entry in Links)
            {
                foreach (var subentry in entry.Value)
                {
                    try {
                        LinksActive.Add(Datafields[entry.Key], Datafields[subentry]);
                    } catch {

                    }
                }
            }
        }

        public void Start()
        {
            foreach (var entry in Datafields)
            {
                entry.Value.Start();
            }
        }

        public System.Collections.Generic.List<string> UpdateImage()
        {
            var result = new System.Collections.Generic.List<string>();

            foreach (var entry in Datafields)
            {
                entry.Value.WertLesen();
                if (entry.Value.aenderung)
                {
                    result.Add(entry.Key);
                }
            }

            return result;
        }

        public void PublishImage()
        {
            foreach (var entry in Datafields)
            {
                entry.Value.WertSchreiben();
            }
        }
    }

    public class var_int : Datafield
    {
        //public new System.Int32 value;

        public var_int(System.Int32 value, String Identifikation, String path = "") : base(Identifikation)
        {
            this.value = value;
            if (path != "") this.path = path;
        }

        public override void WertLesenSpezifisch(System.IO.BinaryReader reader)
        {
            var tmp = reader.ReadInt32();
            if (tmp != (System.Int32)value)
            {
                value = tmp;
                aenderung = true;
            }
        }

        public override void WertSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {
            writer.Write((System.Int32)value);
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

        public static bool operator ==(var_int left, var_int right)
        {
            return (System.Int32)left.value == (System.Int32)right.value;
        }

        public static bool operator !=(var_int left, var_int right)
        {
            return (System.Int32)left.value != (System.Int32)right.value;
        }

        public static bool operator ==(var_int left, System.Int32 right)
        {
            return (System.Int32)left.value == right;
        }

        public static bool operator !=(var_int left, System.Int32 right)
        {
            return (System.Int32)left.value != right;
        }
    }
    
    public class Datafield
    {
        public string Identifikation;
        [Newtonsoft.Json.JsonIgnore]
        private System.Threading.Mutex mutex;
        public object value;
        [Newtonsoft.Json.JsonIgnore]
        public bool aenderung;
        [Newtonsoft.Json.JsonIgnore]
        public string path;
        [Newtonsoft.Json.JsonIgnore]
        public bool write_flag;        
        public Datatype type;

        public Datafield()
        {

        }

        public Datafield(string Identifikation)
        {
            this.Identifikation = Identifikation;
        }

        public void Start()
        {
            ImSpeicherAnlegen();
            MitSpeicherVerknuepfen();
            WertLesen();
        }

        public Datafield(Newtonsoft.Json.Linq.JObject jObject)
        {
            Identifikation = jObject.SelectToken("Identifikation").ToString();
            type = jObject.SelectToken("type").ToObject<Datatype>();

            try {
                var value = jObject.SelectToken("Wert");
                WertAusJToken(value);
            } catch {

            }
        }

        public static Datafield DatafieldTyped(Newtonsoft.Json.Linq.JObject jObject)
        {
            var df_ = new Datafield(jObject);

            switch(df_.type)
            {
                case Datatype.Int32: return jObject.ToObject<var_int>();
            }

            return null;
        }

        public void ImSpeicherAnlegen()
        {
            try
            {
                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                mutex = new System.Threading.Mutex(false, (path + "_mutex").Replace('/', '.'));

                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path).Close();
                    WertSchreiben();
                }
            }
            catch
            {

            }
        }

        public void MitSpeicherVerknuepfen()
        {
            try
            {
                mutex = System.Threading.Mutex.OpenExisting((path + "_mutex").Replace('/', '.'));
            }
            catch
            {
                
            }
        }

        public void MutexBlockierenSynchron()
        {
            try
            {
                mutex.WaitOne();
            }
            catch
            {

            }            
        }

        public void MutexFreigeben()
        {
            mutex.ReleaseMutex();
        }

        public void WertLesen()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            var reader = new System.IO.BinaryReader(stream);

            try {
                //var zst = reader.ReadInt64();
                WertLesenSpezifisch(reader);
            } catch {

            }

            reader.Close();
            stream.Close();

            MutexFreigeben();
        }

        public virtual void WertLesenSpezifisch(System.IO.BinaryReader reader)
        {

        }

        public virtual void WertSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {

        }

        public void WertSchreiben()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Truncate);
            var writer = new System.IO.BinaryWriter(stream);

            WertSchreibenSpezifisch(writer);

            writer.Close();
            stream.Close();
            
            MutexFreigeben();
        }

        public virtual string WertSerialisieren()
        {
            return string.Empty;
        }

        public virtual void WertAusString(System.String s) {}

        public virtual void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token) {}

        public void WertAusJToken(Newtonsoft.Json.Linq.JToken token)
        {
            WertAusJTokenSpezifisch(token);
        }
    }

    public enum Datatype
    {
        Int32
    }
}