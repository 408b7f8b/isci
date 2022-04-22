using System;

namespace library
{
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
        private string Identifikation;
        private System.Threading.Mutex mutex;
        public object value;
        public bool aenderung;

        public Datafield(string Identifikation)
        {
            this.Identifikation = Identifikation;
            ImSpeicherAnlegen();
            MitSpeicherVerknuepfen();
            WertLesen();
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
    }
}
