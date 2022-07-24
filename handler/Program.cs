using System;
using System.Linq;
using library;
using System.Collections.Generic;

namespace handler
{
    public class Ziel : Modell
    {
        public string Adresse, User, Passwort;
    }

    class Program
    {
        static System.Net.Http.HttpClient client;

        static async System.Threading.Tasks.Task GetToFile(string pfad, string ziel)
        {
        // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                System.Net.Http.HttpResponseMessage response = await client.GetAsync(pfad);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                System.IO.File.WriteAllText(ziel, responseBody);
                Console.WriteLine(responseBody);
            }
            catch(System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        static async System.Threading.Tasks.Task PostToFile(string pfad, string stringcontent)
        {
        // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                System.Net.Http.HttpContent content = new System.Net.Http.StringContent(stringcontent, System.Text.Encoding.UTF8, "application/json");
                System.Net.Http.HttpResponseMessage response = await client.PostAsync(pfad, content);
                var s = response.RequestMessage.Content.ToString();
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch(System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        static async System.Threading.Tasks.Task Post(string pfad)
        {
        // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                System.Net.Http.HttpResponseMessage response = await client.PostAsync(pfad, new System.Net.Http.StringContent(""));
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch(System.Net.Http.HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }
        
        static Dictionary<string, List<string>> Umgesetzt = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> Umzusetzen = new Dictionary<string, List<string>>();
        static Dictionary<string, List<string>> Entfernen = new Dictionary<string, List<string>>();
        static List<Ziel> Automatisierungsressourcen = new List<Ziel>();
        static string Adresse;

        static void Main(string[] args)
        {
            var handler = new System.Net.Http.HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback += 
                (sender, certificate, chain, errors) =>
                {
                    return true;
                };

            client = new System.Net.Http.HttpClient(handler);

            while(true)
            {
                try
                {
                    Adresse = System.IO.File.ReadAllText("Plattform.json");
                } catch {

                }

                try
                {
                    var f = System.IO.File.ReadAllText("Automatisierungsressourcen.json");
                    Automatisierungsressourcen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Ziel>>(f);
                } catch {

                }

                if (Automatisierungsressourcen.Count > 0)
                {
                    try
                    {
                        var f = System.IO.File.ReadAllText("Umgesetzt.json");
                        Umgesetzt = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(f);
                    } catch {

                    }

                    for (int i = Umgesetzt.Count-1; i >= 0; --i)
                    {
                        var keys = Umgesetzt.Keys;
                        var keysList = keys.ToList<string>();
                        if (Automatisierungsressourcen.FindAll(a => a.Identifikation == keysList[i]).Count == 0)
                        {
                            Umgesetzt.Remove(keysList[i]);
                        }
                    }

                    var jobj = new Newtonsoft.Json.Linq.JObject();
                    foreach (var ziel in Automatisierungsressourcen)
                    {
                        var get = GetToFile(Adresse + "/Anweisungen/" + ziel.Identifikation + ".json", ziel.Identifikation + ".json");
                        //get.Start();
                        get.Wait();
                        jobj.Merge(Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(ziel.Identifikation + ".json")));
                    }
                    Umzusetzen = jobj.ToObject<Dictionary<string, List<string>>>();

                    var differenz = new Dictionary<string, List<string>>();
                    foreach (var Schritt in Umzusetzen)
                    {
                        if (!Umgesetzt.ContainsKey(Schritt.Key))
                        {
                            differenz.Add(Schritt.Key, Schritt.Value);
                        } else {
                            differenz.Add(Schritt.Key, new List<string>());
                            foreach (var Subschritt in Schritt.Value)
                            {
                                if (!Umgesetzt[Schritt.Key].Contains(Subschritt))
                                {
                                    differenz[Schritt.Key].Add(Subschritt);
                                }
                            }
                        }
                    }

                    foreach (var Schritt in Umgesetzt)
                    {
                        if (!Umzusetzen.ContainsKey(Schritt.Key))
                        {
                            Entfernen.Add(Schritt.Key, Schritt.Value);
                        } else {
                            Entfernen.Add(Schritt.Key, new List<string>());
                            foreach (var Subschritt in Schritt.Value)
                            {
                                if (!Umzusetzen[Schritt.Key].Contains(Subschritt))
                                {
                                    Entfernen[Schritt.Key].Add(Subschritt);
                                }
                            }
                        }
                    }

                    foreach (var diff in differenz)
                    {
                        var Automatisierungsressource = Automatisierungsressourcen.Find(a => a.Identifikation == diff.Key);
                        foreach (var subdiff in diff.Value)
                        {
                            var instanz_task = GetToFile(Adresse + "/Anwendungen/" + subdiff + ".json", subdiff + ".json");
                            //instanz_task.Start();
                            instanz_task.Wait();

                            var instanz = Newtonsoft.Json.JsonConvert.DeserializeObject<Anwendungen.Instanz>(System.IO.File.ReadAllText(subdiff + ".json"));
                            var beschr = new Beschreibung.Automatisierungssystem();
                            beschr.Identifikation = instanz.Identifikation;
                            beschr.Name = instanz.Name;
                            
                            var anwendungen = new List<string>();
                            anwendungen.InsertRange(0, instanz.ReferenzierteModelle);
                            foreach (var basis in anwendungen)
                            {
                                if (!System.IO.File.Exists(basis + ".json"))
                                {
                                    var neu_task = GetToFile(Adresse + "/Anwendungen/" + basis + ".json", basis + ".json");
                                    //neu_task.Start();
                                    neu_task.Wait();
                                }
                                var neu = Newtonsoft.Json.JsonConvert.DeserializeObject<Anwendungen.Basis>(System.IO.File.ReadAllText(basis + ".json"));
                                foreach (var neu_anwendungen in neu.ReferenzierteModelle)
                                {
                                    if (!anwendungen.Contains(neu_anwendungen)) anwendungen.Add(neu_anwendungen);
                                }
                            }

                            foreach (var basis in anwendungen)
                            {
                                var neu = Newtonsoft.Json.JsonConvert.DeserializeObject<Anwendungen.Basis>(System.IO.File.ReadAllText(basis + ".json"));

                                var pseudonyme_basis = instanz.Ressourcenkartierung[neu.Identifikation];

                                var pseudonyme = new List<string>();

                                foreach (var eintrag in pseudonyme_basis)
                                {
                                    if ((eintrag.Value == Automatisierungsressource.Identifikation || eintrag.Value == "*") && !pseudonyme.Contains(eintrag.Key))
                                    {
                                        pseudonyme.Add(eintrag.Key);
                                    }
                                }
                                
                                foreach (var pseudonym in pseudonyme)
                                {
                                    if (!neu.Konfigurationspakete.ContainsKey(pseudonym)) continue;

                                    foreach (var paket_ in neu.Konfigurationspakete[pseudonym])
                                    {
                                        var datei = GetToFile(Adresse + "/Konfigurationen/" + paket_.Value + ".json", paket_.Key + ".json");
                                        //datei.Start();
                                        datei.Wait();
                                        var paket = Newtonsoft.Json.JsonConvert.DeserializeObject<Anwendungen.Konfigurationspaket>(System.IO.File.ReadAllText(paket_.Key + ".json"));

                                        foreach (var element in paket.Elemente)
                                        {
                                            switch (element.Typ)
                                            {
                                                case "Datei": {
                                                    var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Datei>();
                                                    var anforderung = new Dictionary<string, string>();
                                                    anforderung.Add("Cmd", "anlegen");
                                                    anforderung.Add("Url", vorgang.Quelle + "/" + vorgang.Name);
                                                    anforderung.Add("Identifikation", $"{instanz.Identifikation}/Anwendungen/{basis}/{paket_.Key}/{vorgang.Ordner}/{vorgang.Name}");
                                                    var post = PostToFile(Automatisierungsressource.Adresse + "/Applications/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                    //post.Start();
                                                    post.Wait();
                                                    break;
                                                }
                                                case "Service": {
                                                    var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Dienst>();
                                                    var anforderung = new Dictionary<string, string>();
                                                    anforderung.Add("Cmd", vorgang.Operation);
                                                    switch (vorgang.Operation)
                                                    {
                                                        case "create":
                                                        anforderung.Add("Identifikation", $"{instanz.Identifikation}.{basis}.{paket_.Key}.{vorgang.Name}");
                                                        anforderung.Add("Pfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{basis}/{paket_.Key}/{vorgang.Ziel}").Replace("//", "/"));
                                                        anforderung.Add("Arbeitspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{basis}/{paket_.Key}/{vorgang.Arbeitspfad}").Replace("//", "/"));
                                                        break;
                                                        case "enable":
                                                        case "disable":
                                                        case "start":
                                                        case "stop":
                                                        case "restart":
                                                        anforderung.Add("Identifikation", $"{instanz.Identifikation}.{basis}.{paket_.Key}.{vorgang.Name}");
                                                        break;
                                                        case "reload":
                                                        break; 
                                                    }
                                                    var post = PostToFile(Automatisierungsressource.Adresse + "/Services/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                    //post.Start();
                                                    post.Wait();
                                                    break;
                                                }
                                                case "Parameter":
                                                {
                                                    var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Parameter>();
                                                    var anforderung = new Dictionary<string, object>();
                                                    anforderung.Add("Cmd", "variablen");
                                                    anforderung.Add("Variablen", vorgang.Variablen);
                                                    anforderung.Add("Rekursiv", "true");
                                                    anforderung.Add("Bezugspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{basis}/{paket_.Key}/{vorgang.Ordner}").Replace("//", "/"));
                                                    var post = PostToFile(Automatisierungsressource.Adresse + "/Parameter/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                    post.Wait();
                                                    break;
                                                }                                                    
                                            }
                                        }
                                    }
                                }

                                foreach (var pseudonym in pseudonyme)
                                {
                                    if (!neu.Konfigurationselemente.ContainsKey(pseudonym)) continue;

                                    foreach (var element in neu.Konfigurationselemente[pseudonym])
                                    {
                                        switch (element.Typ)
                                        {
                                            case "Datei": {
                                                var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Datei>();
                                                var anforderung = new Dictionary<string, string>();
                                                anforderung.Add("Cmd", "anlegen");
                                                anforderung.Add("Url", vorgang.Quelle + "/" + vorgang.Name);
                                                anforderung.Add("Identifikation", $"{instanz.Identifikation}/Anwendungen/{basis}/{vorgang.Ordner}/{vorgang.Name}");
                                                var post = PostToFile(Automatisierungsressource.Adresse + "/Applications/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                //post.Start();
                                                post.Wait();
                                                break;
                                            }
                                            case "Service": {
                                                var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Dienst>();
                                                var anforderung = new Dictionary<string, string>();
                                                anforderung.Add("Cmd", vorgang.Operation);
                                                switch (vorgang.Operation)
                                                {
                                                    case "create":
                                                    anforderung.Add("Identifikation", $"{instanz.Identifikation}.{basis}.{vorgang.Name}");
                                                    anforderung.Add("Pfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{basis}/{vorgang.Ziel}").Replace("//", "/"));
                                                    anforderung.Add("Arbeitspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{basis}/{vorgang.Arbeitspfad}").Replace("//", "/"));
                                                    break;
                                                    case "enable":
                                                    case "disable":
                                                    case "start":
                                                    case "stop":
                                                    case "restart":
                                                    anforderung.Add("Identifikation", $"{instanz.Identifikation}.{basis}.{vorgang.Name}");
                                                    break;
                                                    case "reload":
                                                    break; 
                                                }
                                                var post = PostToFile(Automatisierungsressource.Adresse + "/Services/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                //post.Start();
                                                post.Wait();
                                                break;
                                            }
                                            case "Parameter":
                                            {
                                                var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Parameter>();
                                                var anforderung = new Dictionary<string, object>();
                                                anforderung.Add("Cmd", "variablen");
                                                anforderung.Add("Variablen", vorgang.Variablen);
                                                anforderung.Add("Rekursiv", "true");
                                                anforderung.Add("Bezugspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{basis}/{vorgang.Ordner}").Replace("//", "/"));
                                                var post = PostToFile(Automatisierungsressource.Adresse + "/Parameter/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                post.Wait();
                                                break;
                                            }                                                    
                                        }
                                    }
                                }
                            }

                            var pseudonyme_instanz = instanz.Ressourcenkartierung[instanz.Identifikation];

                            var pseudonyme_final = new List<string>();

                            foreach (var eintrag in pseudonyme_instanz)
                            {
                                if (eintrag.Value == Automatisierungsressource.Identifikation && !pseudonyme_final.Contains(eintrag.Key))
                                {
                                    pseudonyme_final.Add(eintrag.Key);
                                }
                            }

                            foreach (var pseudonym in pseudonyme_final)
                            {
                                if (!instanz.Konfigurationspakete.ContainsKey(pseudonym)) continue;

                                foreach (var paket_ in instanz.Konfigurationspakete[pseudonym])
                                {
                                    var datei = GetToFile(Adresse + "/Konfigurationen/" + paket_.Value + ".json", paket_.Key + ".json");
                                    datei.Start();
                                    datei.Wait();
                                    var paket = Newtonsoft.Json.JsonConvert.DeserializeObject<Anwendungen.Konfigurationspaket>(System.IO.File.ReadAllText(paket_.Key + ".json"));

                                    foreach (var element in paket.Elemente)
                                    {
                                        switch (element.Typ)
                                        {
                                            case "Datei": {
                                                var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Datei>();
                                                var anforderung = new Dictionary<string, string>();
                                                anforderung.Add("Cmd", "anlegen");
                                                anforderung.Add("Url", vorgang.Quelle + "/" + vorgang.Name);
                                                anforderung.Add("Identifikation", $"{instanz.Identifikation}/Anwendungen/{paket_.Key}/{vorgang.Ordner}/{vorgang.Name}");
                                                var post = PostToFile(Automatisierungsressource.Adresse + "/Applications/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                //post.Start();
                                                post.Wait();
                                                break;
                                            }
                                            case "Service": {
                                                var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Dienst>();
                                                var anforderung = new Dictionary<string, string>();
                                                anforderung.Add("Cmd", vorgang.Operation);
                                                switch (vorgang.Operation)
                                                {
                                                    case "create":
                                                    anforderung.Add("Identifikation", $"{instanz.Identifikation}.{paket_.Key}.{vorgang.Name}");
                                                    anforderung.Add("Pfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{paket_.Key}/{vorgang.Ziel}").Replace("//", "/"));
                                                    anforderung.Add("Arbeitspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{paket_.Key}/{vorgang.Arbeitspfad}").Replace("//", "/"));
                                                    break;
                                                    case "enable":
                                                    case "disable":
                                                    case "start":
                                                    case "stop":
                                                    case "restart":
                                                    anforderung.Add("Identifikation", $"{instanz.Identifikation}.{paket_.Key}.{vorgang.Name}");
                                                    break;
                                                    case "reload":
                                                    break; 
                                                }
                                                var post = PostToFile(Automatisierungsressource.Adresse + "/Services/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                //post.Start();
                                                post.Wait();
                                                break;
                                            }
                                            case "Parameter":
                                            {
                                                var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Parameter>();
                                                var anforderung = new Dictionary<string, object>();
                                                anforderung.Add("Cmd", "variablen");
                                                anforderung.Add("Variablen", vorgang.Variablen);
                                                anforderung.Add("Rekursiv", "true");
                                                anforderung.Add("Bezugspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{paket_.Key}/{vorgang.Ordner}").Replace("//", "/"));
                                                var post = PostToFile(Automatisierungsressource.Adresse + "/Parameter/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                                post.Wait();
                                                break;
                                            }                                                    
                                        }
                                    }
                                }
                            }

                            foreach (var pseudonym in pseudonyme_final)
                            {
                                if (!instanz.Konfigurationselemente.ContainsKey(pseudonym)) continue;

                                foreach (var element in instanz.Konfigurationselemente[pseudonym])
                                {
                                    switch (element.Typ)
                                    {
                                        case "Datei": {
                                            var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Datei>();
                                            var anforderung = new Dictionary<string, string>();
                                            anforderung.Add("Cmd", "anlegen");
                                            anforderung.Add("Url", vorgang.Quelle + "/" + vorgang.Name);
                                            anforderung.Add("Identifikation", $"{instanz.Identifikation}/Anwendungen/{vorgang.Ordner}/{vorgang.Name}");
                                            var post = PostToFile(Automatisierungsressource.Adresse + "/Applications/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                            //post.Start();
                                            post.Wait();
                                            break;
                                        }
                                        case "Service": {
                                            var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Dienst>();
                                            var anforderung = new Dictionary<string, string>();
                                            anforderung.Add("Cmd", vorgang.Operation);
                                            switch (vorgang.Operation)
                                            {
                                                case "create":
                                                anforderung.Add("Identifikation", $"{instanz.Identifikation}.{vorgang.Name}");
                                                anforderung.Add("Pfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{vorgang.Ziel}").Replace("//", "/"));
                                                anforderung.Add("Arbeitspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{vorgang.Arbeitspfad}").Replace("//", "/"));
                                                break;
                                                case "enable":
                                                case "disable":
                                                case "start":
                                                case "stop":
                                                case "restart":
                                                anforderung.Add("Identifikation", $"{instanz.Identifikation}.{vorgang.Name}");
                                                break;
                                                case "reload":
                                                break; 
                                            }
                                            var post = PostToFile(Automatisierungsressource.Adresse + "/Services/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                            //post.Start();
                                            post.Wait();
                                            break;
                                        }
                                        case "Parameter":
                                        {
                                            var vorgang = ((Newtonsoft.Json.Linq.JObject)element.Vorgang).ToObject<Anwendungen.Konfigurationselement.Parameter>();
                                            var anforderung = new Dictionary<string, object>();
                                            anforderung.Add("Cmd", "variablen");
                                            anforderung.Add("Variablen", vorgang.Variablen);
                                            anforderung.Add("Rekursiv", "true");
                                            anforderung.Add("Bezugspfad", ($"$APPLICATIONS/{instanz.Identifikation}/Anwendungen/{vorgang.Ordner}").Replace("//", "/"));
                                            var post = PostToFile(Automatisierungsressource.Adresse + "/Parameter/anforderung", Newtonsoft.Json.JsonConvert.SerializeObject(anforderung));
                                            post.Wait();
                                            break;
                                        }                                                    
                                    }
                                }
                            }
                        }
                    }

                    var ser = Newtonsoft.Json.JsonConvert.SerializeObject(Umzusetzen);
                    System.IO.File.WriteAllText("Umgesetzt.json", ser);
                    Umgesetzt = Umzusetzen;
                }

                System.Threading.Thread.Sleep(10000);
            }            
        }
    }
}
