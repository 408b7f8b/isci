using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using Fraunhofer.IPA.MSB.Client.API.Attributes;
using Fraunhofer.IPA.MSB.Client.API.Configuration;
using Fraunhofer.IPA.MSB.Client.API.Model;
using Fraunhofer.IPA.MSB.Client.Websocket;

namespace msb
{
    class modulkonfiguration : library.Konfiguration
    {
        public string MsbWebsocketUrl;
        public string MsbSmartObjectUuid;
        public string MsbSmartObjectToken;

        public modulkonfiguration(string datei) : base(datei)
        {

        }
    }

    class Program
    {
        /*public static void callbackFunction(params object[] obj)
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
                        case library.Datatype.Int32: {
                            df.value = (System.Int32)obj[i]; break;
                        }
                    }
                }

                update = true;
            } catch {

            }
        }*/

        public class MsbSmartObject : SmartObject
        {
            public MsbSmartObject(string uuid, string name, string description, string token) : base(uuid, name, description, token)
            {

            }

            public new void AddFunction(Function function)
            {

            }
        }

        public class MsbFunction : Function
        {
            //public delegate void FunctionPointerType (params object[] obj);
            //public new FunctionPointerType FunctionPointer;

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
                            case library.Datatype.Int32: {
                                df.value = (System.Int32)obj[i]; break;
                            }
                        }
                    }

                    update = true;
                } catch {

                }
            }

            /*public void callbackFunction(Dictionary<string, object> parameter)
            {
                var info = (FunctionCallInfo)parameter["functionCallInfo"];

                try {
                    foreach (var param in parameter)
                    {
                        if (param.Key == "functionCallInfo") continue;

                        var df = Funktionen[info.Function.Id].First(f => f.Identifikation == param.Key);

                        switch (df.type)
                        {
                            case library.Datatype.Int32: {
                                df.value = (System.Int32)param.Value; break;
                            }
                        }
                    }

                    update = true;
                } catch {

                }
            }*/

            public MsbFunction(library.Funktion archFunktion)
            {
                Id = archFunktion.Identifikation;
                Name = archFunktion.Name;
                Description = archFunktion.Beschreibung;
                ResponseEventIds = new List<string>();
                var mInfo = typeof(MsbFunction).GetMethod("callbackFunction");
                this.FunctionPointer = CreateFunctionPointer(archFunktion, mInfo, this);
                this.DataFormat = this.CreateDataFormatFromMethodInfo(archFunktion, mInfo);
            }

            private Delegate CreateFunctionPointer(library.Funktion archFunktion, MethodInfo methodInfo, object callableObjectForMethod)
            {
                /*var parameter = new System.Type[archFunktion.Datenfelder.Count()+1];
                int i = 0;
                foreach (var datenfeld in archFunktion.Datenfelder)
                {
                    switch (structure.Datafields[datenfeld].type)
                    {
                        case library.Datatype.Int32:
                        {
                            parameter[i] = typeof(System.Int32); break;
                        }
                    }
                    ++i;
                }
                parameter[archFunktion.Datenfelder.Count()] = typeof(FunctionCallInfo);*/

                //return new FunctionPointerType(this.callbackFunction);

                var functionParameterTypes = from parameter in methodInfo.GetParameters() select parameter.ParameterType;

                var delgateType = System.Linq.Expressions.Expression.GetActionType(functionParameterTypes.ToArray());

                return methodInfo.CreateDelegate(delgateType, callableObjectForMethod);
            }

            private Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat CreateDataFormatFromMethodInfo(library.Funktion archFunktion, MethodInfo methodInfo)
            {
                var parameter = new System.Type[archFunktion.Datenfelder.Count()];
                int i = 0;
                foreach (var datenfeld in archFunktion.Datenfelder)
                {
                    switch (structure.Datafields[datenfeld].type)
                    {
                        case library.Datatype.Int32:
                        {
                            parameter[i] = typeof(System.Int32); break;
                        }
                    }
                    ++i;
                }

                var msbFunctionDataFormat = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat();

                for (i = 0; i < parameter.Count(); ++i)
                {
                    var dataFormatOfParameter = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat(archFunktion.Datenfelder[i], parameter[i]);
                    foreach (var subDataFormat in dataFormatOfParameter)
                    {
                        msbFunctionDataFormat.Add(subDataFormat.Key, subDataFormat.Value);
                    }
                }

                return msbFunctionDataFormat;
            }
        }       

        static Dictionary<string, List<library.Datafield>> Funktionen = new Dictionary<string, List<library.Datafield>>();
        static bool update = false;
        static library.Datastructure structure;

        static void Main(string[] args)
        {
            var konfiguration = new modulkonfiguration("konfiguration_msb.json");
            structure = new library.Datastructure(konfiguration.OrdnerDatenstruktur);

            var files = System.IO.Directory.GetFiles(konfiguration.OrdnerDatenmodelle, "*.json");

            foreach (var file in files)
            {
                structure.AddDatamodel(library.Datamodel.FromFile(file));
            }
            structure.Start();

            files = System.IO.Directory.GetFiles(konfiguration.OrdnerBeschreibungen, "*.json");

            var beschreibungen = new Newtonsoft.Json.Linq.JObject();
            foreach (var file in files)
            {
                var tmp_file = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(file));
                beschreibungen.Merge(tmp_file);
            }

            // Create a new MsbClient which allows SmartObjects and Applications to communicate with the MSB
            var MsbClient = new MsbClient(konfiguration.MsbWebsocketUrl);

            var MsbSmartObject = new SmartObject(konfiguration.MsbSmartObjectUuid, konfiguration.Anwendung, beschreibungen.ToString().Replace("/", "//").Replace("\"", "\\\""), konfiguration.MsbSmartObjectToken);

            var ereignisse = new Dictionary<library.Datafield, List<Event>>();
            var dateninhalte = new Dictionary<Event, List<library.Datafield>>();

            files = System.IO.Directory.GetFiles(konfiguration.OrdnerEreignismodelle, "*.json");
            foreach (var file in files)
            {
                var inh = System.IO.File.ReadAllText(file);
                var liste = Newtonsoft.Json.JsonConvert.DeserializeObject<List<library.Ereignis>>(inh);
                foreach (var ereignis in liste)
                {
                    if (!structure.Datafields.ContainsKey(ereignis.Ausloeser)) continue;

                    var df = structure.Datafields[ereignis.Ausloeser];
                    
                    if (ereignisse.Where(a => a.Key.Identifikation == ereignis.Ausloeser).Count() == 0) ereignisse.Add(df, new List<Event>());
                    
                    if (ereignisse[df].Where(a => a.Id == ereignis.Identifikation).Count() != 0) continue;

                    var parameter = new System.Type[ereignis.Datenfelder.Count()];
                    int i = 0;
                    foreach (var datenfeld in ereignis.Datenfelder)
                    {
                        switch (structure.Datafields[datenfeld].type)
                        {
                            case library.Datatype.Int32:
                            {
                                parameter[i] = typeof(System.Int32); break;
                            }
                        }
                        ++i;
                    }

                    var msbEventDataFormat = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat();
                    msbEventDataFormat.Add("dataObject", new Dictionary<string, object>(){{"$ref", "#/definitions/fields"}});
                    var fields = new Dictionary<string, object>();
                    fields.Add("type", "object");
                    fields.Add("properties", new Dictionary<string, object>());
                    for (i = 0; i < ereignis.Datenfelder.Count(); ++i)
                    {
                        var dataFormatOfParameter = new Fraunhofer.IPA.MSB.Client.Websocket.Model.DataFormat(ereignis.Datenfelder[i], parameter[i]);
                        foreach (var subDataFormat in dataFormatOfParameter)
                        {
                            ((Dictionary<string, object>)fields["properties"]).Add(subDataFormat.Key, subDataFormat.Value);
                            //msbEventDataFormat.Add(subDataFormat.Key, subDataFormat.Value);
                        }
                    }
                    msbEventDataFormat.Add("fields", fields);

                    var msbEvent = new Event(ereignis.Identifikation, ereignis.Name, ereignis.Beschreibung, msbEventDataFormat);
                    ereignisse[df].Add(msbEvent);
                    MsbSmartObject.AddEvent(msbEvent);

                    dateninhalte.Add(msbEvent, new List<library.Datafield>());
                    foreach (var datenfeld in ereignis.Datenfelder)
                    {
                        switch (structure.Datafields[datenfeld].type)
                        {
                            case library.Datatype.Int32:
                            {
                                dateninhalte[msbEvent].Add(structure.Datafields[datenfeld]); break;
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
                var liste = Newtonsoft.Json.JsonConvert.DeserializeObject<List<library.Funktion>>(inh);
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

            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            while(true)
            {
                Zustand.WertLesen();
                var erfüllteTransitionen = konfiguration.Zustandsbereiche.Where(a => a.Arbeitszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<library.Zustandsbereich>() > 0)
                {
                    if (update)
                    {
                        structure.PublishImage();
                        update = false;
                    }

                    structure.UpdateImage();
                    foreach (var datenfeld in ereignisse)
                    {
                        if (datenfeld.Key.aenderung)
                        {
                            var werte = new Dictionary<string, object>();

                            foreach (var ereignis in datenfeld.Value)
                            {
                                foreach (var eintrag in dateninhalte[ereignis])
                                {
                                    werte.Add(eintrag.Identifikation, eintrag.value);
                                }

                                var eventData = new EventDataBuilder(ereignis)
                                .SetValue(werte)
                                .Build();

                                MsbClient.PublishAsync(MsbSmartObject, eventData).Wait();
                            }

                            datenfeld.Key.aenderung = false;
                        }
                    }
                    
                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}