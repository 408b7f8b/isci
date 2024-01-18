using System;
using System.Linq;

namespace isci.Daten
{
    public class Dateneintrag
    {
        public string Identifikation;
        //[Newtonsoft.Json.JsonIgnore]
        //private System.Threading.Mutex mutex;
        public object value;
        [Newtonsoft.Json.JsonIgnore]
        public bool aenderung;
        [Newtonsoft.Json.JsonIgnore]
        public string path;
        [Newtonsoft.Json.JsonIgnore]
        public bool write_flag;
        [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public Datentypen type;
        public bool istListe;
        public UInt16 listeDimensionen;
        //public System.Collections.Generic.List<string> components;
        public string parentEintrag; //ist für den Bau von Bäumen
        [Newtonsoft.Json.JsonIgnore]
        public const uint size = 4;

        //parent, children? oder relationen?
        //dimensionen array?

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
            WertAusSpeicherLesen();
        }

        public Dateneintrag(Newtonsoft.Json.Linq.JObject jObject)
        {
            Identifikation = jObject.SelectToken("Identifikation").ToString();
            type = jObject.SelectToken("type").ToObject<Datentypen>();
            if (jObject.ContainsKey("istListe")) istListe = jObject.SelectToken("istListe").ToObject<bool>();
            if (jObject.ContainsKey("listeDimensionen")) listeDimensionen = jObject.SelectToken("listeDimensionen").ToObject<UInt16>();
            if (jObject.ContainsKey("parentEintrag")) parentEintrag = jObject.SelectToken("parentEintrag").ToString();

            try
            {
                var value = jObject.SelectToken("Wert");
                WertAusJToken(value);
            }
            catch
            {

            }
        }

        public System.Type GibTyp()
        {
            switch (this.type)
            {
                case Datentypen.UInt8: return typeof(sbyte);
                case Datentypen.UInt16: return typeof(UInt16);
                case Datentypen.UInt32: return typeof(UInt32);
                case Datentypen.Int8: return typeof(byte);
                case Datentypen.Int16: return typeof(Int16);
                case Datentypen.Int32: return typeof(Int32);
                case Datentypen.Float: return typeof(float);
                case Datentypen.Double: return typeof(double);
                case Datentypen.Bool: return typeof(bool);
            }

            return null;
        }

        public static Dateneintrag GibDateneintragTypisiert(Newtonsoft.Json.Linq.JObject jObject)
        {
            var df_ = new Dateneintrag(jObject);

            switch (df_.type)
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

            }
            catch
            {

            }

            //mutex
            /*try
            {
                mutex = new System.Threading.Mutex(false, (path + "_mutex").Replace('/', '.'));
            } catch {

            }*/

            try
            {
                if (!System.IO.File.Exists(path))
                {
                    System.IO.File.Create(path).Close();
                    WertInSpeicherSchreiben();
                }
            }
            catch
            {
                Console.WriteLine("ImSpeicherAnlagen fehlgeschlagen: " + this.Identifikation);
            }
        }

        public void MitSpeicherVerknuepfen()
        {
            //mutex
            /*
            try
            {
                mutex = System.Threading.Mutex.OpenExisting((path + "_mutex").Replace('/', '.'));
            }
            catch
            {
                Console.WriteLine("MitSpeicherVerknuepfen fehlgeschlagen: " + this.Identifikation);
            }*/
        }

        public void MutexBlockierenSynchron()
        {
            //mutex
            /*
            try
            {
                mutex.WaitOne();
            }
            catch
            {
                Console.WriteLine("MutexBlockierenSynchron fehlgeschlagen: " + this.Identifikation);
            } */
        }

        public void MutexFreigeben()
        {
            //mutex
            //mutex.ReleaseMutex();
        }

        public void WertAusSpeicherLesen()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            var reader = new System.IO.BinaryReader(stream);

            try
            {
                //var zst = reader.ReadInt64();
                WertAusSpeicherLesenSpezifisch(reader);
            }
            catch
            {
                Console.WriteLine("Lesen fehlgeschlagen: " + this.Identifikation);
            }

            reader.Close();
            stream.Close();

            MutexFreigeben();
        }

        public virtual void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {

        }

        public virtual void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {

        }

        public void WertInSpeicherSchreiben()
        {
            MutexBlockierenSynchron();

            var stream = new System.IO.FileStream(path, System.IO.FileMode.Truncate);
            var writer = new System.IO.BinaryWriter(stream);

            WertInSpeicherSchreibenSpezifisch(writer);

            writer.Close();
            stream.Close();

            MutexFreigeben();
        }

        public virtual string WertSerialisieren()
        {
            return string.Empty;
        }

        public virtual void WertAusString(System.String s) { }

        public virtual void WertAusJTokenSpezifisch(Newtonsoft.Json.Linq.JToken token) { }

        public virtual void WertAusBytes(byte[] bytes) { }

        public void WertAusJToken(Newtonsoft.Json.Linq.JToken token)
        {
            WertAusJTokenSpezifisch(token);
        }

        public Newtonsoft.Json.Linq.JToken WertNachJToken()
        {
            return WertNachJTokenSpezifisch();
        }
        public virtual Newtonsoft.Json.Linq.JToken WertNachJTokenSpezifisch() { return null; }

        public virtual byte[] WertNachBytes() { return null; }

        public string DateneintragSerialisiert(bool formatiert = false)
        {
            var alsJson = DateneintragAlsJToken();
            if (formatiert) return alsJson.ToString(Newtonsoft.Json.Formatting.Indented);
            else return alsJson.ToString(Newtonsoft.Json.Formatting.None);
        }

        public virtual Newtonsoft.Json.Linq.JObject DateneintragAlsJToken()
        {
            var alsJson = Newtonsoft.Json.Linq.JObject.FromObject(this);
            return alsJson;
        }

        public string gibName()
        {
            if (!this.Identifikation.StartsWith("ns=3;s=")) return Identifikation;
            if (!this.Identifikation.Contains('.')) return this.Identifikation.Substring(7);

            return this.Identifikation.Substring(this.Identifikation.LastIndexOf('.') + 1);
        }

        public string gibVollenNamen()
        {
            if (!this.Identifikation.StartsWith("ns=3;s=")) return Identifikation;
            else return this.Identifikation.Substring(7);
        }

        public int gibNamensraum()
        {
            var splits = new string[2];
            splits[0] = "ns=";
            splits[1] = ";";
            var teile = this.Identifikation.Split(splits, StringSplitOptions.RemoveEmptyEntries);
            return int.Parse(teile[0]);
        }

        public string gibDatenmodell()
        {
            if (!this.Identifikation.StartsWith("ns=3;s=")) return "";
            if (!this.Identifikation.Contains('.')) return this.Identifikation.Substring(7);

            var teile = this.Identifikation.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (teile.Length > 1) return teile[0];
            else return "";
        }

        public string gibElterneintrag()
        {
            if (this.parentEintrag != null) return this.parentEintrag;

            if (!this.Identifikation.Contains('.')) return this.gibDatenmodell();

            var top = this.Identifikation.Substring(0, this.Identifikation.LastIndexOf('.'));

            return top;
        }
    }
}