using System;
using System.Linq;
using System.Diagnostics;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace abbild
{
    class Program
    {
        public class Konfiguration : library.Konfiguration
        {
            public string token;
            public string adresse;
            public string orgId;
            public string pfad;
            public int abbildlaenge;
            public int pause;

            public Konfiguration(string datei) : base(datei)
            {
            }

            /*public Konfiguration(string datei)
            {
                if (!System.IO.File.Exists(datei))
                {
                    return;
                }
                else
                {
                    try
                    {
                        var t = Newtonsoft.Json.Linq.JToken.Parse(System.IO.File.ReadAllText(datei));
                        Console.WriteLine("Konfigdatei: " + datei);

                        var felder = GetType().GetFields();

                        foreach (var f in felder)
                        {
                            try
                            {
                                var feld = GetType().GetField(f.Name);
                                var feldtyp = feld.FieldType;

                                if (feldtyp == typeof(string))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<string>();
                                    feld.SetValue(this, o);
                                }
                                else if (feldtyp == typeof(int))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<int>();
                                    feld.SetValue(this, o);
                                }
                                else if (feldtyp == typeof(uint))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<uint>();
                                    feld.SetValue(this, o);
                                }
                                else if (feldtyp == typeof(bool))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<bool>();
                                    feld.SetValue(this, o);
                                }
                                else if (feldtyp == typeof(double))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<double>();
                                    feld.SetValue(this, o);
                                }
                                else if (feldtyp == typeof(Int32[]))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<Int32[]>();
                                    feld.SetValue(this, o);
                                }
                                else if (feldtyp == typeof(string[]))
                                {
                                    var o = t.SelectToken(f.Name).ToObject<string[]>();
                                    feld.SetValue(this, o);
                                }

                                try {
                                    Console.WriteLine("Konfigparam " + feld.Name + ": " + feld.GetValue(this).ToString());
                                } catch { }
                            }
                            catch { }
                        }
                    } catch 
                    {

                    }                
                }
            }*/
        }
        
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");

            var beschreibung = new Beschreibung.Modul();
            beschreibung.Identifikation = konfiguration.Identifikation;
            beschreibung.Name = "Abbild Ressource " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Modul zur Abbilderstellung gegen externe Datenbank";
            beschreibung.Typidentifikation = "isci.abbild";
            beschreibung.Datenfelder = new library.FieldList();
            beschreibung.Ereignisse = new System.Collections.Generic.List<library.Ereignis>();
            beschreibung.Funktionen = new System.Collections.Generic.List<library.Funktion>();
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");

            var structure = new library.Datastructure(konfiguration.OrdnerDatenstruktur);
            structure.AddDataModelsFromDirectory(konfiguration.OrdnerDatenmodelle);
            structure.Start();

            var influxDBClient = InfluxDBClientFactory.Create(konfiguration.adresse, konfiguration.token);
            influxDBClient.SetLogLevel(InfluxDB.Client.Core.LogLevel.None);

            var writeOptions = WriteOptions
            .CreateNew()
            .BatchSize(50000)
            .FlushInterval(10000)
            .Build();

            var writeApi = influxDBClient.GetWriteApi(writeOptions);
            var orgname = (await influxDBClient.GetOrganizationsApi().FindOrganizationByIdAsync(konfiguration.orgId)).Name;
            
            var bucketApi = influxDBClient.GetBucketsApi();
            var bucketApiOrg = (await bucketApi.FindBucketsByOrgNameAsync(orgname));

            var buckets = new System.Collections.Generic.List<string>();
            foreach (var bucket in bucketApiOrg)
            {
                buckets.Add(bucket.Name);
            }

            if (!buckets.Contains(konfiguration.Anwendung))
            {
                bucketApi.CreateBucketAsync(new Bucket(name:konfiguration.Anwendung, orgID:konfiguration.orgId,
                retentionRules:new System.Collections.Generic.List<BucketRetentionRules>(){
                    new BucketRetentionRules(BucketRetentionRules.TypeEnum.Expire, 0, 3600)
                })).Wait();
            }

            var abbild = new System.Collections.Generic.List<string>();

            /*if (!System.IO.File.Exists("written")) {
                System.IO.File.Create("written").Close();
            }

            var written = System.IO.File.ReadAllLines("written").ToList<string>();*/

            while(true)
            {
                /*var files = System.IO.Directory.GetFiles(konfiguration.pfad);

                foreach (var f in files)
                {
                    if (!written.Contains(f))
                    {
                        var f_split = f.Substring(f.LastIndexOf('/')+1).Split('_');
                        var lprotocol = System.IO.File.ReadAllLines(f);
                        if (!buckets.Contains(f_split[0]))
                        {
                            bucketApi.CreateBucketAsync(new Bucket(name:f_split[0], orgID:konfiguration.orgId)).RunSynchronously();
                            buckets.Add(f_split[0]);
                        }
                        writeApi.WriteRecords(lprotocol, WritePrecision.Ms, bucket:f_split[0], org:konfiguration.orgId);
                        written.Add(f);
                        System.IO.File.WriteAllLines("written", written);
                    }
                }*/

                structure.UpdateImage();
                var dt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();

                foreach (var feld in structure.Datafields)
                {
                    var l = $"{feld.Key},ressource={konfiguration.Ressource} value={feld.Value.WertSerialisieren()} {dt}";
                    abbild.Add(l);
                }

                if (abbild.Count > konfiguration.abbildlaenge)
                {
                    writeApi.WriteRecords(abbild, bucket:konfiguration.Anwendung, org:konfiguration.orgId, precision:WritePrecision.Ms);
                    abbild.Clear();
                }

                System.Threading.Thread.Sleep(konfiguration.pause);
            }
        }
    }
}
