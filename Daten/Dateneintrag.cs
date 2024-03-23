using System;
using System.Collections;
using System.Linq;

namespace isci.Daten
{
    public class Dateneintrag
    {
        public string Identifikation;
        //[Newtonsoft.Json.JsonIgnore]
        //private System.Threading.Mutex mutex;
        protected object Wert_;
        public virtual object Wert
        {
            get
            {
                return Wert_;
            }
            set
            {
                Wert_ = value;
                this.aenderungIntern = true;
            }
        }

        protected bool aenderungExtern_;
        [Newtonsoft.Json.JsonIgnore]
        public bool aenderungExtern
        {
            get
            {
                return this.aenderungExtern_;
            }
            set
            {
                this.aenderungExtern_ = value;
                if (value) this.aenderungIntern = false;
            }
        }
        
        [Newtonsoft.Json.JsonIgnore]
        public bool aenderungIntern;
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

        //parent, children? oder relationen?
        //dimensionen array?

        public EinheitenKodierung Einheit = EinheitenKodierung.keine;

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
            if (jObject.ContainsKey("Einheit")) Einheit = (isci.Daten.EinheitenKodierung)jObject.SelectToken("Einheit").ToObject<int>();

            try
            {
                var Wert = jObject.SelectToken("Wert");
                WertAusJToken(Wert);
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
                case Datentypen.UInt64: return typeof(UInt64);
                case Datentypen.Int8: return typeof(byte);
                case Datentypen.Int16: return typeof(Int16);
                case Datentypen.Int32: return typeof(Int32);
                case Datentypen.Int64: return typeof(Int64);
                case Datentypen.Float: return typeof(float);
                case Datentypen.Double: return typeof(double);
                case Datentypen.Bool: return typeof(bool);
                case Datentypen.String: return typeof(string);
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
                case Datentypen.UInt64: return jObject.ToObject<dtUInt64>();
                case Datentypen.Int8: return jObject.ToObject<dtInt8>();
                case Datentypen.Int16: return jObject.ToObject<dtInt16>();
                case Datentypen.Int32: return jObject.ToObject<dtInt32>();
                case Datentypen.Int64: return jObject.ToObject<dtInt64>();
                case Datentypen.Float: return jObject.ToObject<dtFloat>();
                case Datentypen.Double: return jObject.ToObject<dtDouble>();
                case Datentypen.Bool: return jObject.ToObject<dtBool>();
                case Datentypen.String: return jObject.ToObject<dtString>();
                case Datentypen.Objekt: return jObject.ToObject<dtObjekt>();
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
            catch (System.Exception e)
            {
                Logger.Fatal("Ausnahme beim Anlegen des Dateneintrags " + this.Identifikation + ": " + e.Message);
                System.Environment.Exit(-1);
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
            catch (System.Exception e)
            {
                Logger.Fehler("Ausnahme beim Anlegen des Dateneintrags " + this.Identifikation + ": " + e.Message);
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

            System.IO.FileStream stream = null;
            System.IO.BinaryReader reader = null;

            try
            {
                stream = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite);
                reader = new System.IO.BinaryReader(stream);
                WertAusSpeicherLesenSpezifisch(reader);
            }
            catch
            {
                Logger.Fehler("WertAusSpeicherLesen fehlgeschlagen: " + this.Identifikation);
            }
            finally {
                if (reader != null) reader.Dispose();
                if (stream != null) stream.Dispose();
            }

            MutexFreigeben();
        }

        public virtual void WertAusSpeicherLesenSpezifisch(System.IO.BinaryReader reader)
        {

        }

        public virtual void WertInSpeicherSchreibenSpezifisch(System.IO.BinaryWriter writer)
        {

        }

        public void WertInSpeicherSchreiben(bool force = false)
        {
            if (!aenderungIntern && !force) return;

            MutexBlockierenSynchron();

            System.IO.FileStream stream = null;
            System.IO.BinaryWriter writer = null;

            try
            {
                stream = new System.IO.FileStream(path, System.IO.FileMode.Truncate, System.IO.FileAccess.ReadWrite, System.IO.FileShare.ReadWrite);
                writer = new System.IO.BinaryWriter(stream);
                WertInSpeicherSchreibenSpezifisch(writer);
            }
            catch
            {
                Logger.Fehler("WertInSpeicherSchreiben fehlgeschlagen: " + this.Identifikation);
            }
            finally
            {
                if (writer != null) writer.Dispose();
                if (stream != null) stream.Dispose();
            }

            MutexFreigeben();

            aenderungIntern = false;
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

        /* public string gibName()
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
        }*/

        public string gibElterneintrag()
        {
            if (this.parentEintrag != null) return this.parentEintrag;
            return "";
        }

        /// <summary>
        /// Methode zur Korrektur der eigenen Identifikation nach dem Muster 'anwendung.datenmodell.spezifischeIdentifikationDateneintrag'.
        /// </summary>
        /// <param name="datenmodell">Korrekte Identifikation des Datenmodells.</param>
        /// <param name="anwendung">Korrekte Identifikation der Anwendung.</param>
        public void korrigiereIdentifikationFallsNotw(string datenmodell, string anwendung = null)
        {
            string correctedString = this.Identifikation;
            datenmodell = datenmodell.Replace(" ", "");

            if (parentEintrag != "" && parentEintrag != null)
            {
                if (!correctedString.StartsWith(parentEintrag))
                {
                    if (correctedString.Contains('.'))
                    {
                        correctedString = $"{parentEintrag}.{correctedString.Substring(correctedString.LastIndexOf('.') + 1)}";
                    }
                    else
                    {
                        correctedString = $"{parentEintrag}.{correctedString}";
                    }
                }
            }

            if (anwendung == null)
            {
                if (!correctedString.StartsWith($"{datenmodell}."))
                {
                    /* if (correctedString.Contains('.'))
                    {
                        correctedString = $"{datenmodell}.{correctedString.Substring(correctedString.LastIndexOf('.')+1)}";
                    }
                    else
                    { */
                        correctedString = $"{datenmodell}.{correctedString}";
                    /* } */
                }
            } else {
                anwendung = anwendung.Replace(" ", "");

                if (anwendung == "")
                {
                    if (!correctedString.StartsWith($"{datenmodell}."))
                    {
                        /* if (correctedString.Contains('.'))
                    {
                        correctedString = $"{datenmodell}.{correctedString.Substring(correctedString.LastIndexOf('.')+1)}";
                    }
                    else
                    { */
                        correctedString = $"{datenmodell}.{correctedString}";
                        /* } */
                    }
                } else {
                    if (correctedString.StartsWith($"{anwendung}"))
                    {
                        

                    } else {
                        if (correctedString.StartsWith($"{datenmodell}"))
                        {
                            correctedString = $"{anwendung}.{correctedString}";
                        } else {
                            correctedString = $"{anwendung}.{datenmodell}.{correctedString}";
                        }
                    }
                }
            }

            this.Identifikation = correctedString;
        }

        public T TypisiertAls<T>() {

            if (
                typeof(T) == typeof(dtUInt8) ||
                typeof(T) == typeof(dtUInt16) ||
                typeof(T) == typeof(dtUInt32) ||
                typeof(T) == typeof(dtUInt64) ||
                typeof(T) == typeof(dtInt8) ||
                typeof(T) == typeof(dtInt16) ||
                typeof(T) == typeof(dtInt32) ||
                typeof(T) == typeof(dtInt64) ||
                typeof(T) == typeof(dtDouble) ||
                typeof(T) == typeof(dtFloat) ||
                typeof(T) == typeof(dtString) ||
                typeof(T) == typeof(dtZustand) ||
                typeof(T) == typeof(dtObjekt) ||
                typeof(T) == typeof(dtBool)
            )
            {
                return (T)this.AlsObjekt(typeof(T));
            }

            return default(T);
        }

        public object AlsObjekt(Type objectType)
        {
            if (objectType == typeof(dtUInt8))
            {
                return (dtUInt8)this;
            } else if (objectType == typeof(dtUInt16))
            {
                return (dtUInt16)this;
            } else if (objectType == typeof(dtUInt32))
            {
                return (dtUInt32)this;
            } else if (objectType == typeof(dtUInt64))
            {
                return (dtUInt64)this;
            } else if (objectType == typeof(dtInt8))
            {
                return (dtInt8)this;
            } else if (objectType == typeof(dtInt16))
            {
                return (dtInt16)this;
            } else if (objectType == typeof(dtInt32))
            {
                return (dtInt32)this;
            } else if (objectType == typeof(dtInt64))
            {
                return (dtInt64)this;
            } else if (objectType == typeof(dtDouble))
            {
                return (dtDouble)this;
            } else if (objectType == typeof(dtFloat))
            {
                return (dtFloat)this;
            } else if (objectType == typeof(dtString))
            {
                return (dtString)this;
            } else if (objectType == typeof(dtZustand))
            {
                return (dtZustand)this;
            } else if (objectType == typeof(dtObjekt))
            {
                return (dtObjekt)this;
            } else if (objectType == typeof(dtBool))
            {
                return (dtBool)this;
            }

            return null;
        }
    }
}