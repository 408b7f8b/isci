using System;
using System.Linq;
using System.Diagnostics;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;

namespace example
{
    class Program
    {
        public class Konfiguration
        {
            public string token;
            public string adresse;
            public string orgId;
            public string pfad;

            public Konfiguration(string datei)
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
            }
        }
        
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");

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

            if (!System.IO.File.Exists("written")) {
                System.IO.File.Create("written").Close();
            }

            var written = System.IO.File.ReadAllLines("written").ToList<string>();

            while(true)
            {
                var files = System.IO.Directory.GetFiles(konfiguration.pfad);

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
                }
                System.Threading.Thread.Sleep(2000);
            }
        }
    }
}
