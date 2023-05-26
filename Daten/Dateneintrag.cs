using System;
using System.Linq;

namespace isci.Daten
{   
    public class Dateneintrag
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
        public Datentypen type;
        public bool istListe;

        public Dateneintrag()
        {

        }

        public Dateneintrag(string Identifikation)
        {
            this.Identifikation = Identifikation;
        }

        public void Start()
        {
            ImSpeicherAnlegen();
            MitSpeicherVerknuepfen();
            Lesen();
        }

        public Dateneintrag(Newtonsoft.Json.Linq.JObject jObject)
        {
            Identifikation = jObject.SelectToken("Identifikation").ToString();
            type = jObject.SelectToken("type").ToObject<Datentypen>();

            try {
                var value = jObject.SelectToken("Wert");
                AusJToken(value);
            } catch {

            }
        }

        public static Dateneintrag DatafieldTyped(Newtonsoft.Json.Linq.JObject jObject)
        {
            var df_ = new Dateneintrag(jObject);

            switch(df_.type)
            {
                case Datentypen.UInt8: return jObject.ToObject<dtUInt8>();
                case Datentypen.UInt16: return jObject.ToObject<dtUInt16>();
                case Datentypen.UInt32: return jObject.ToObject<dtUInt32>();
                case Datentypen.Int8: return jObject.ToObject<dtInt8>();
                case Datentypen.Int16: return jObject.ToObject<dtInt16>();
                case Datentypen.Int32: return jObject.ToObject<dtInt32>();
                case Datentypen.Float: return jObject.ToObject<dtFloat>();
                case Datentypen.Double: return jObject.ToObject<dtDouble>();
                case Datentypen.Bool: return jObject.ToObject<dtBool>();
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

            } catch {

            }

            try
            {
                mutex = new System.Threading.Mutex(false, (path + "_mutex").Replace('/', '.'));
            } catch {

            }
            
            try
            {
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path).Close();
                    Schreiben();
                }
            }
            catch
            {
                Console.WriteLine("ImSpeicherAnlagen fehlgeschlagen: " + this.Identifikation);
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
                Console.WriteLine("MitSpeicherVerknuepfen fehlgeschlagen: " + this.Identifikation);
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
                Console.WriteLine("MutexBlockierenSynchron fehlgeschlagen: " + this.Identifikation);
            }            
        }

        public void MutexFreigeben()
        {
            mutex.ReleaseMutex();
        }

        public void Lesen()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            var reader = new System.IO.BinaryReader(stream);

            try {
                //var zst = reader.ReadInt64();
                LesenSpezifisch(reader);
            } catch {
                Console.WriteLine("Lesen fehlgeschlagen: " + this.Identifikation);
            }

            reader.Close();
            stream.Close();

            MutexFreigeben();
        }

        public virtual void LesenSpezifisch(System.IO.BinaryReader reader)
        {

        }

        public virtual void SchreibenSpezifisch(System.IO.BinaryWriter writer)
        {

        }

        public void Schreiben()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Truncate);
            var writer = new System.IO.BinaryWriter(stream);

            SchreibenSpezifisch(writer);

            writer.Close();
            stream.Close();
            
            MutexFreigeben();
        }

        public virtual string Serialisieren()
        {
            return string.Empty;
        }

        public virtual void AusString(System.String s) {}

        public virtual void AusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token) {}

        public void AusJToken(Newtonsoft.Json.Linq.JToken token)
        {
            AusJTokenSpezifisch(token);
        }

        public Newtonsoft.Json.Linq.JToken NachJToken()
        {
            return NachJTokenSpezifisch();
        }
        public virtual Newtonsoft.Json.Linq.JToken NachJTokenSpezifisch() { return null; }

        public string getName()
        {
            if (!this.Identifikation.StartsWith("ns=2;s=")) return Identifikation;
            if (!this.Identifikation.Contains('.')) return this.Identifikation.Substring(7);

            return this.Identifikation.Substring(this.Identifikation.LastIndexOf('.') + 1);
        }

        public string getFullname()
        {
            if (!this.Identifikation.StartsWith("ns=2;s=")) return Identifikation;
            else return this.Identifikation.Substring(7);
        }

        public int getNamespace()
        {
            var splits = new string[2];
            splits[0] = "ns=";
            splits[1] = ";";
            var teile = this.Identifikation.Split(splits, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(teile[0]);
        }

        public string getDatenmodell()
        {
            if (!this.Identifikation.StartsWith("ns=2;s=")) return "";
            if (!this.Identifikation.Contains('.')) return this.Identifikation.Substring(7);

            var teile = this.Identifikation.Split(new char[]{'.'}, StringSplitOptions.RemoveEmptyEntries);
            if (teile.Length > 1) return teile[0];
            else return "";
        }

        public string getTop()
        {
            if (!this.Identifikation.Contains('.')) return this.getDatenmodell();

            var top = this.Identifikation.Substring(0, this.Identifikation.LastIndexOf('.'));
            
            return top;
        }
    }
}