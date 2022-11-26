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
        public bool istLIste;

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
                case Datentypen.Int32: return jObject.ToObject<dtInt32>();
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
                    Schreiben();
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

        public void Lesen()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            var reader = new System.IO.BinaryReader(stream);

            try {
                //var zst = reader.ReadInt64();
                LesenSpezifisch(reader);
            } catch {

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
    }
}