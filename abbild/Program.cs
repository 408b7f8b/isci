using System;
using System.Linq;
using System.Diagnostics;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace example
{
    class Program
    {
        public class abbildkonfiguration : library.Konfiguration
        {
            public string token;
            public string adresse;
            public string orgId;
            public string bucket;

            public abbildkonfiguration(string datei) : base(datei)
            {

            }
        }

        public static long GetNanoseconds()
        {
            double timestamp = Stopwatch.GetTimestamp();
            double nanoseconds = 1_000_000_000.0 * timestamp / Stopwatch.Frequency;

            return (long)nanoseconds;
        }

        static void Main(string[] args)
        {
            var konfiguration = new abbildkonfiguration("konfiguration.json");

            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen + "/" + konfiguration.Anwendungen[0]);
            structure.AddDataModelsFromDirectory(konfiguration.OrdnerAnwendungen + "/" + konfiguration.Anwendungen[0]);
            structure.Start();

            var Zustand = new library.var_int(0, konfiguration.OrdnerDatenstrukturen + "/" + konfiguration.Anwendungen[0] + "/Zustand");
            Zustand.Start();

            var influxDBClient = InfluxDBClientFactory.Create(konfiguration.adresse, konfiguration.token);                

            var writeOptions = WriteOptions
            .CreateNew()
            .BatchSize(50000)
            .FlushInterval(10000)
            .Build();

            while(true)
            {
                Zustand.WertLesen();
                if (Zustand.value == konfiguration.Zustandsbereiche[0].Arbeitszustand)
                {                
                    structure.UpdateImage();
                    
                    using (var writeApi = influxDBClient.GetWriteApi(writeOptions))
                    {
                        var timestamp = " " + GetNanoseconds().ToString();
                        foreach (var entry in structure.Datafields)
                        {
                            writeApi.WriteRecord(entry.Value.Identifikation + " value=" + entry.Value.WertSerialisieren() + timestamp, WritePrecision.Ns, konfiguration.bucket, konfiguration.orgId);
                        }                    
                    }

                    structure.PublishImage();
                    Zustand.value = konfiguration.Zustandsbereiche[0].Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}
