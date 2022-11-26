using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Fraunhofer.IPA.MSB.Client.API.Model;
using Fraunhofer.IPA.MSB.Client.Websocket;
using isci.Allgemein;
using isci.Daten;
using isci.Anwendungen;
using isci.Beschreibung;

namespace isci.msb
{
    class modulkonfiguration : Parameter
    {
        public string MsbWebsocketUrl;
        public string MsbSmartObjectUuid;
        public string MsbSmartObjectToken;

        public modulkonfiguration(string datei) : base(datei) {}
    }

    class Program
    {
        public class MsbSmartObject : SmartObject
        {
            public MsbSmartObject(string uuid, string name, string description, string token) : base(uuid, name, description, token) {}

            public new void AddFunction(Function function) {}
        }

        public class MsbFunction : Function
        {
            public void callbackFunction(params object[] obj)
            {
                var mn = obj[0].GetType().Name;
                var info = (FunctionCallInfo)obj[obj.Length-1];

                try {
                    for(int i = 0; i < obj.Length-1; ++i)
                    {
                        if (obj[i] == null) continue;

                        var df = Funktionen[info.Function.Id][i];

                        switch (df.type)
                        {
                            case Datentypen.Int32: {
                                df.value = (System.Int32)obj[i]; break;
                            }
                        }
                    }

                    update = true;
                } catch {}
            }

            public MsbFunction(Funktion archFunktion)
            {
                Id = archFunktion.Identifikation;
                Name = archFunktion.Name;
                Description = archFunktion.Beschreibung;
                ResponseEventIds = new List<string>();
                var mInfo = typeof(MsbFunction).GetMethod("callbackFunction");
                this.FunctionPointer = CreateFunctionPointer(archFunktion, mInfo, this);
                this.DataFormat = this.CreateDataFormatFromMethodInfo(archFunktion, mInfo);
            }

            private Delegate CreateFunctionPointer(Funktion archFunktion, MethodInfo methodInfo, object callableObjectForMethod)
            {
                var functionParameterTypes = from parameter in methodInfo.GetParameters() select parameter.ParameterType;
                var delgateType = System.Linq.Expressions.Expression.GetActionType(functionParameterTypes.ToArray());

                return methodInfo.CreateDelegate(delgateType, callableObjectForMethod);
            }

            private Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat CreateDataFormatFromMethodInfo(Funktion archFunktion, MethodInfo methodInfo)
            {
                var parameter = new System.Type[archFunktion.Ziele.Count()];
                int i = 0;
                foreach (var datenfeld in archFunktion.Ziele)
                {
                    switch (struktur.dateneinträge[datenfeld].type)
                    {
                        case Datentypen.Int32:
                        {
                            if (struktur.dateneinträge[datenfeld].istLIste)
                                parameter[i] = typeof(List<System.Int32>);
                            else
                                parameter[i] = typeof(System.Int32); break;
                        }
                    }
                    ++i;
                }

                var msbFunctionDataFormat = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat();

                int list_c = 1;

                for (i = 0; i < parameter.Count(); ++i)
                {
                    var dataFormatOfParameter = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat(archFunktion.Ziele[i], parameter[i], list_c);
                    foreach (var subDataFormat in dataFormatOfParameter)
                    {
                        /*var key_neu = subDataFormat.Key;
                        if (key_neu.Contains("List"))
                        {
                            key_neu = key_neu.Substring(0, 5) + (list_c).ToString();
                        }

                        try {
                            var val = (System.Collections.Generic.KeyValuePair<string, string>)subDataFormat.Value;
                            if (val.Value.Contains("List"))
                            {
                                var val_neu = new System.Collections.Generic.KeyValuePair<string, string>(val.Key, val.Value.Substring(0, val.Value.IndexOf("List")) + "List`" + list_c.ToString());
                                msbFunctionDataFormat.Add(subDataFormat.Key, val_neu);
                            }
                        } catch {*/
                            msbFunctionDataFormat.Add(subDataFormat.Key, subDataFormat.Value);
                            if (subDataFormat.Key.Contains("List")) ++list_c;
                        //}
                    }                        
                }

                return msbFunctionDataFormat;
            }
        }       

        static Dictionary<string, List<Dateneintrag>> Funktionen = new Dictionary<string, List<Dateneintrag>>();
        static bool update = false;
        static Datenstruktur struktur;

        static void Main(string[] args)
        {
            var konfiguration = new modulkonfiguration("konfiguration_msb.json");

            var beschreibung = new Modul(konfiguration.Identifikation, "isci.msb");
            beschreibung.Name = "MSB Ressource " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Modul zur MSB-Integration";
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");

            struktur = new Datenstruktur(konfiguration.OrdnerDatenstruktur);

            var files = System.IO.Directory.GetFiles(konfiguration.OrdnerDatenmodelle, "*.json");

            foreach (var file in files) struktur.DatenmodellEinhängen(Datenmodell.AusDatei(file));
            struktur.Start();

            files = System.IO.Directory.GetFiles(konfiguration.OrdnerBeschreibungen, "*.json");

            var beschreibungen = new Newtonsoft.Json.Linq.JObject();
            foreach (var file in files)
            {
                var tmp_file = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(file));
                beschreibungen.Merge(tmp_file);
            }

            var MsbClient = new MsbClient(konfiguration.MsbWebsocketUrl);

            var beschr = beschreibungen.ToString().Replace("/", "//").Replace("\"", "\\\"");
            var MsbSmartObject = new SmartObject(konfiguration.MsbSmartObjectUuid, konfiguration.Anwendung, konfiguration.Anwendung + " über Ressource " + konfiguration.Ressource, konfiguration.MsbSmartObjectToken);

            var ereignisse = new Dictionary<Dateneintrag, List<Event>>();
            var dateninhalte = new Dictionary<Event, List<Dateneintrag>>();

            files = System.IO.Directory.GetFiles(konfiguration.OrdnerEreignismodelle, "*.json");
            foreach (var file in files)
            {
                var inh = System.IO.File.ReadAllText(file);
                var liste = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ereignis>>(inh);
                foreach (var ereignis in liste)
                {
                    if (!struktur.dateneinträge.ContainsKey(ereignis.Ausloeser)) continue;

                    var df = struktur.dateneinträge[ereignis.Ausloeser];
                    
                    if (ereignisse.Where(a => a.Key.Identifikation == ereignis.Ausloeser).Count() == 0) ereignisse.Add(df, new List<Event>());
                    
                    if (ereignisse[df].Where(a => a.Id == ereignis.Identifikation).Count() != 0) continue;

                    var parameter = new System.Type[ereignis.Elemente.Count()];
                    int i = 0;
                    foreach (var datenfeld in ereignis.Elemente)
                    {
                        switch (struktur.dateneinträge[datenfeld].type)
                        {
                            case Datentypen.Int32:
                            {
                                if (struktur.dateneinträge[datenfeld].istLIste)
                                {
                                    parameter[i] = typeof(List<System.Int32>);
                                } else {
                                    parameter[i] = typeof(System.Int32);
                                }
                                break;
                            }
                        }
                        ++i;
                    }

                    var msbEventDataFormat = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat();
                    msbEventDataFormat.Add("dataObject", new Dictionary<string, object>(){{"$ref", "#/definitions/fields"}});
                    var fields = new Dictionary<string, object>();
                    fields.Add("type", "object");
                    fields.Add("properties", new Dictionary<string, object>());
                    for (i = 0; i < ereignis.Elemente.Count(); ++i)
                    {
                        var dataFormatOfParameter = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat(ereignis.Elemente[i], parameter[i]);
                        foreach (var subDataFormat in dataFormatOfParameter)
                            ((Dictionary<string, object>)fields["properties"]).Add(subDataFormat.Key, subDataFormat.Value);
                    }
                    msbEventDataFormat.Add("fields", fields);

                    var msbEvent = new Event(ereignis.Identifikation, ereignis.Name, ereignis.Beschreibung, msbEventDataFormat);
                    ereignisse[df].Add(msbEvent);
                    MsbSmartObject.AddEvent(msbEvent);

                    dateninhalte.Add(msbEvent, new List<Dateneintrag>());
                    foreach (var datenfeld in ereignis.Elemente)
                    {
                        switch (struktur.dateneinträge[datenfeld].type)
                        {
                            case Datentypen.Int32:
                            {
                                dateninhalte[msbEvent].Add(struktur.dateneinträge[datenfeld]); break;
                            }
                        }
                        ++i;
                    }
                }
            }

            files = System.IO.Directory.GetFiles(konfiguration.OrdnerFunktionsmodelle, "*.json");
            foreach (var file in files)
            {
                var inh = System.IO.File.ReadAllText(file);
                var liste = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Funktion>>(inh);
                foreach (var funktion_ in liste)
                {
                    var funktion = new MsbFunction(funktion_);
                    MsbSmartObject.AddFunction(funktion);
                }                
            }

            // Connect to the MSB and register the sample SmartObject and sample Application via the MsbClient
            MsbClient.ConnectAsync();
            System.Threading.Thread.Sleep(5000);
            MsbClient.RegisterAsync(MsbSmartObject);

            struktur.Start();

            var Zustand = new dtInt32(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            while(true)
            {
                Zustand.Lesen();
                var erfüllteTransitionen = konfiguration.Ausführungstransitionen.Where(a => a.Eingangszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<Ausführungstransition>() > 0)
                {
                    if (update)
                    {
                        struktur.Schreiben();
                        update = false;
                    }

                    struktur.Lesen();
                    foreach (var dateneintrag in ereignisse)
                    {
                        if (dateneintrag.Key.aenderung)
                        {
                            var werte = new Dictionary<string, object>();

                            foreach (var ereignis in dateneintrag.Value)
                            {
                                foreach (var eintrag in dateninhalte[ereignis])
                                    werte.Add(eintrag.Identifikation, eintrag.value);

                                var eventData = new EventDataBuilder(ereignis)
                                .SetValue(werte)
                                .Build();

                                MsbClient.PublishAsync(MsbSmartObject, eventData).Wait();
                            }

                            dateneintrag.Key.aenderung = false;
                        }
                    }
                    
                    Zustand.value = erfüllteTransitionen.First<Ausführungstransition>().Ausgangszustand;
                    Zustand.Schreiben();
                }
            }
        }
    }
}