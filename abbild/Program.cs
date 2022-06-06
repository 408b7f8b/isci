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

            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0]);
            structure.AddDataModelsFromDirectory(konfiguration.OrdnerDatenmodelle[0]);
            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0] + "/Zustand");
            Zustand.Start();

            var influxDBClient = InfluxDBClientFactory.Create(konfiguration.adresse, konfiguration.token);
            influxDBClient.SetLogLevel(InfluxDB.Client.Core.LogLevel.None);

            var writeOptions = WriteOptions
            .CreateNew()
            .BatchSize(50000)
            .FlushInterval(10000)
            .Build();

            var writeApi = influxDBClient.GetWriteApi(writeOptions);

            while(true)
            {
                Zustand.WertLesen();
                var erfüllteTransitionen = konfiguration.Zustandsbereiche.Where(a => a.Arbeitszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<library.Zustandsbereich>() > 0)
                {
                    structure.UpdateImage();
                    
                    //using ()
                    //{
                        var timestamp = DateTime.UtcNow;
                        System.Collections.Generic.List<InfluxDB.Client.Writes.PointData> points = new System.Collections.Generic.List<InfluxDB.Client.Writes.PointData>();
                        
                        foreach (var entry in structure.Datafields)
                        {
                            var point = InfluxDB.Client.Writes.PointData.Measurement(entry.Value.Identifikation)
                                        .Tag("Ressource", konfiguration.Ressource)
                                        .Field("value", entry.Value.value)
                                        .Timestamp(timestamp, WritePrecision.Ms);
                            points.Add(point);
                            //writeApi.WritePoint(point, konfiguration.bucket, konfiguration.orgId);
                        }
                        writeApi.WritePoints(points, konfiguration.bucket, konfiguration.orgId);
                    //}

                    System.Threading.Thread.Sleep(10);

                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}
