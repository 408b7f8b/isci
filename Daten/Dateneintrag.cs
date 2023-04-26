﻿using System;
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
                case Datentypen.UInt16: return jObject.ToObject<dtUInt16>();
                case Datentypen.Int16: return jObject.ToObject<dtInt16>();
                case Datentypen.Int32: return jObject.ToObject<dtInt32>();
                case Datentypen.Float: return jObject.ToObject<dtFloat>();
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
    }
}