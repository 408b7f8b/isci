using System;
using System.Linq;
using System.Diagnostics;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace abbilddb
{
    class Program
    {
        public class abbildkonfiguration : library.Konfiguration
        {
            public string token;
            public string adresse;
            public string orgId;
            public string bucket;
            public int ZyklenJeAbbild;

            public abbildkonfiguration(string datei) : base(datei)
            {

            }
        }
        
        static void Main(string[] args)
        {
            var konfiguration = new abbildkonfiguration("konfiguration.json");

            var Abbilder = konfiguration.OrdnerDatenstrukturen + "/" + konfiguration.OrdnerAnwendungen[0] + "/Strukturabbild";

            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0]);
            structure.AddDataModelsFromDirectory(konfiguration.OrdnerDatenmodelle[0]);
            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0] + "/Zustand");
            Zustand.Start();

            /*var influxDBClient = InfluxDBClientFactory.Create(konfiguration.adresse, konfiguration.token);
            influxDBClient.SetLogLevel(InfluxDB.Client.Core.LogLevel.None);

            var writeOptions = WriteOptions
            .CreateNew()
            .BatchSize(50000)
            .FlushInterval(10000)
            .Build();

            var writeApi = influxDBClient.GetWriteApi(writeOptions);*/

            System.Collections.Generic.List<string> lprotocol = new System.Collections.Generic.List<string>();

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
                    //System.Collections.Generic.List<InfluxDB.Client.Writes.PointData> points = new System.Collections.Generic.List<InfluxDB.Client.Writes.PointData>();
                    
                    foreach (var entry in structure.Datafields)
                    {
                        var point = InfluxDB.Client.Writes.PointData.Measurement(entry.Value.Identifikation)
                                    .Tag("Ressource", konfiguration.Ressource)
                                    .Field("value", entry.Value.value)
                                    .Timestamp(timestamp, WritePrecision.Ms);
                        var s = point.ToLineProtocol();

                        //points.Add(point);
                        lprotocol.Add(s);
                        //writeApi.WritePoint(point, konfiguration.bucket, konfiguration.orgId);
                    }

                    if (lprotocol.Count == konfiguration.ZyklenJeAbbild)
                    {
                        System.IO.File.WriteAllLines(Abbilder, lprotocol);
                        lprotocol.Clear();
                    }

                    //writeApi.WriteRecords(lprotocol, WritePrecision.Ms, bucket:konfiguration.bucket, org:konfiguration.orgId);
                    //writeApi.WritePoints(points, konfiguration.bucket, konfiguration.orgId);
                    //}

                    //Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}
