using System;
using System.Linq;
using isci.Beschreibung;
using System.Collections.Generic;

namespace handler.http
{
    public class Funktionen
    {
        public static void RessourceBeschreiben(string Ressource, string adresse)
        {
            var b = new Automatisierungsressource();
        }

        public static Newtonsoft.Json.Linq.JObject Beschreiben(string adresse)
        {
            var handler = new System.Net.Http.HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback += 
                (sender, certificate, chain, errors) =>
                {
                    return true;
                };

            var client = new System.Net.Http.HttpClient(handler);

            var resp = client.GetAsync(adresse + "/Applications/Ordner");
            resp.Wait();
            var cont = resp.Result.Content.ReadAsStringAsync();
            cont.Wait();
            var inh = cont.Result;

            var instanzen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(inh);

            var gobj = new Newtonsoft.Json.Linq.JObject();
            foreach (var instanz in instanzen)
            {
                var resp_b = client.GetAsync(adresse + "/Applications/Dateien/" + instanz + "/Beschreibungen");
                resp_b.Wait();
                cont = resp.Result.Content.ReadAsStringAsync();
                cont.Wait();
                var beschreibungen = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(cont.Result);
                //var obj = new Newtonsoft.Json.Linq.JObject();
                foreach (var beschreibung in beschreibungen)
                {
                    var resp_d = client.GetAsync(adresse + "/Applications/" + instanz + "/Beschreibungen/" + beschreibung);
                    resp_d.Wait();
                    cont = resp_d.Result.Content.ReadAsStringAsync();
                    cont.Wait();
                    //var subobj = Newtonsoft.Json.Linq.JObject.Parse(cont.Result);
                    //obj.Merge(obj);
                    var modul = Newtonsoft.Json.JsonConvert.DeserializeObject<Modul>(cont.Result);
                }
                //gobj.Add(instanz, obj);
            }
            return gobj;
        }
    }
}