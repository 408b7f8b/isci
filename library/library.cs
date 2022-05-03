using System;

namespace library
{
    public class Datamodel
    {
        public string Identifikation;
        public System.Collections.Generic.List<Datafield> Datafields;

        public Datamodel()
        {
            Datafields = new System.Collections.Generic.List<Datafield>();
        }
        public Datamodel(string Identifikation)
        {
            this.Identifikation = Identifikation;
            Datafields = new System.Collections.Generic.List<Datafield>();
        }

        public static Datamodel FromFile(string path)
        {
            var file = System.IO.File.ReadAllText(path);
            var datamodel_ = Newtonsoft.Json.Linq.JObject.Parse(file);

            var datamodel = AusJObject(datamodel_);

            return datamodel;
        }

        public string ToString()
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
            dm.Datafields = new System.Collections.Generic.List<Datafield>();

            var entries = datamodel_.SelectToken("Datafields");

            foreach (Newtonsoft.Json.Linq.JObject entry in entries)
            {
                var de = Datafield.DatafieldTyped(entry);
                dm.Datafields.Add(de);
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

        public Datastructure()
        {
            Datamodels = new System.Collections.Generic.List<string>();
            Datafields = new System.Collections.Generic.Dictionary<string, Datafield>();
        }

        public void AddDatamodel(Datamodel datamodel)
        {
            foreach (var entry in datamodel.Datafields)
            {
                string ident = "ns=" + datamodel.Identifikation + ";s=" + entry.Identifikation;
                entry.Identifikation = ident;
                Datafields.Add(ident, entry);
            }
            Datamodels.Add(datamodel.Identifikation);
        }

        public void AddDatamodelFromFile(string path)
        {
            var dm_ = Datamodel.FromFile(path);
            AddDatamodel(dm_);
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
        public new System.Int32 value;

        public var_int(System.Int32 value, String Identifikation) : base(Identifikation)
        {
            this.value = value;
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

        public static bool operator ==(var_int left, var_int right)
        {
            return left.value == right.value;
        }

        public static bool operator !=(var_int left, var_int right)
        {
            return left.value != right.value;
        }

        public static bool operator ==(var_int left, System.Int32 right)
        {
            return left.value == right;
        }

        public static bool operator !=(var_int left, System.Int32 right)
        {
            return left.value != right;
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
                var dir = System.IO.Path.GetDirectoryName(Identifikation);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                mutex = new System.Threading.Mutex(false, (Identifikation + "_mutex").Replace('/', '.'));

                if (!System.IO.File.Exists(Identifikation))
                {
                    System.IO.File.Create(Identifikation).Close();
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
                mutex = System.Threading.Mutex.OpenExisting((Identifikation + "_mutex").Replace('/', '.'));
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

            var stream = new System.IO.FileStream(Identifikation, System.IO.FileMode.Open);
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

            var stream = new System.IO.FileStream(Identifikation, System.IO.FileMode.Truncate);
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
