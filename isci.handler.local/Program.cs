using System;
using System.Linq;
using isci;
using isci.Anwendungen;
using isci.Beschreibung;
using isci.Konfiguration;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace handler.local
{
    public class Konfiguration
    {
        public string IdentifikationHandler;
        public string Plattform;
        public string AUTOMATISIERUNG_RESSOURCE;
        public string AUTOMATISIERUNG_DATENSTRUKTUREN;
        public string AUTOMATISIERUNG_ANWENDUNGEN;
        public Konfiguration(string datei) {
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

                            try {
                                Console.WriteLine("Konfigparam " + feld.Name + ": " + feld.GetValue(this).ToString());
                            } catch { }
                        }
                        catch { }
                    }
                } catch { }
            }
        }

        public void createEnv()
        {
            createEnv("AUTOMATISIERUNG_RESSOURCE", AUTOMATISIERUNG_RESSOURCE);
            createEnv("AUTOMATISIERUNG_DATENSTRUKTUREN", AUTOMATISIERUNG_DATENSTRUKTUREN);
            createEnv("AUTOMATISIERUNG_ANWENDUNGEN", AUTOMATISIERUNG_ANWENDUNGEN);
        }

        public void createEnv(string envVariable, string value)
        {
            try {
                if (Environment.GetEnvironmentVariable(envVariable) == null )
                {
                    Environment.SetEnvironmentVariable(envVariable, value);
                    var bashrc = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.bashrc";
                    var bashrc_lines = System.IO.File.ReadAllLines(bashrc).ToList<string>();
                    bool flag = false;

                    for (int i = 0; i < bashrc_lines.Count(); ++i)
                    {
                        if (bashrc_lines[i].StartsWith(envVariable + "="))
                        {
                            bashrc_lines[i] = $"{envVariable}=\"{value}\"";
                            break;
                        }
                    }

                    if (!flag)
                    {
                        bashrc_lines.Add($"{envVariable}=\"{value}\"");
                    }

                    System.IO.File.WriteAllLines(bashrc, bashrc_lines);
                }
            } catch {

            }
        }
    }

    class Program
    {
        static HttpClient client;

        static async System.Threading.Tasks.Task GetToFile(string pfad, string ziel)
        {
            try
            {
                var response = await client.GetAsync(pfad);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var ziel_ordner = System.IO.Path.GetDirectoryName(ziel);
                if (!System.IO.Directory.Exists(ziel_ordner))
                {
                    System.IO.Directory.CreateDirectory(ziel_ordner);
                }
                System.IO.File.WriteAllText(ziel, responseBody);
                Console.WriteLine(responseBody);
            }
            catch(System.Exception e)
            {
                Console.WriteLine("\nException Caught!");	
                Console.WriteLine("Message :{0} ",e.Message);
            }
        }

        public static void Action(string filename, string arguments)
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = filename;           
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        static void AnwendungInstallieren(string anwendung, string anwendungsordner, string ressource_identifikation)
        {


            var instanz_objekt = JsonConvert.DeserializeObject<Instanz>(System.IO.File.ReadAllText(anwendung + ".json"));

            AnwendungInstallieren(instanz_objekt, anwendungsordner, ressource_identifikation);

        }

        static void AnwendungInstallieren(Instanz anwendung, string anwendungsordner, string ressource_identifikation)
        {
            var basisanwendungen = new List<string>();
            basisanwendungen.InsertRange(0, anwendung.ReferenzierteModelle);

            foreach (var basisanwendung in basisanwendungen)
            {
                var neu_task = GetToFile(Adresse + "/Anwendungen/" + basisanwendung + ".json", basisanwendung + ".json");
                neu_task.Wait();
                var basisanwendung_objekt = JsonConvert.DeserializeObject<Basis>(System.IO.File.ReadAllText(basisanwendung + ".json"));
                foreach (var neue_anwendung in basisanwendung_objekt.ReferenzierteModelle)
                {
                    if (!basisanwendungen.Contains(neue_anwendung)) basisanwendungen.Add(neue_anwendung);
                }
            }            

            foreach (var basisanwendung in basisanwendungen)
            {
                var pseudonyme_basis = anwendung.Ressourcenkartierung[basisanwendung];
                var pseudonyme = new List<string>();

                foreach (var eintrag in pseudonyme_basis)
                {
                    if ((eintrag.Value == ressource_identifikation || eintrag.Value == "*") && !pseudonyme.Contains(eintrag.Key))
                    {
                        pseudonyme.Add(eintrag.Key);
                    }
                }

                AnwendungInstallieren(basisanwendung, anwendung.Identifikation, pseudonyme, anwendungsordner);
            }

            if (anwendung.Konfigurationspakete.ContainsKey(ressource_identifikation))
            {
                foreach (var paket_ in anwendung.Konfigurationspakete[ressource_identifikation])
                {
                    var datei = GetToFile(Adresse + "/Konfigurationen/" + paket_.Value + ".json", "tmp/" + paket_.Key + ".json");
                    datei.Wait();
                    var paket = JsonConvert.DeserializeObject<Konfigurationspaket>(System.IO.File.ReadAllText("tmp/" + paket_.Key + ".json"));

                    foreach (var element in paket.Elemente)
                    {
                        switch (element.typ)
                        {
                            case "Datei": {
                                var vorgang = (Datei)element.vorgang;//((JObject)element.vorgang).ToObject<Datei>();
                                var datei_task = GetToFile(vorgang.Quelle + "/" + vorgang.Name, $"{anwendungsordner}/{anwendung.Identifikation}/{paket_.Key}/{vorgang.Ordner}/{vorgang.Name}");
                                datei_task.Wait();
                                break;
                            }
                            case "Service": {
                                var vorgang = (Dienst)element.vorgang;//((JObject)element.vorgang).ToObject<Dienst>();
                                var servicename = $"{anwendung.Identifikation}.{paket_.Key}.{vorgang.Name}";
                                var systemd_user = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";

                                switch (vorgang.Operation)
                                {
                                    case "create":
                                    var arbeitspfad = ($"{anwendungsordner}/{anwendung.Identifikation}/{paket_.Key}/{vorgang.Arbeitspfad}").Replace("//", "/");
                                    var pfad = ($"{anwendungsordner}/{anwendung.Identifikation}/{paket_.Key}/{vorgang.Ziel}").Replace("//", "/");

                                    string Beschreibung = $"[Unit]\n" +
                                    $"Description={servicename}\n\n" +
                                    "[Service]\n" +
                                    $"WorkingDirectory={arbeitspfad}\n" +
                                    $"ExecStart={pfad}\n" +
                                    "Restart=always\n\n" +
                                    "[Install]\n" +
                                    "WantedBy=default.target";

                                    System.IO.File.WriteAllText(systemd_user + "/" + servicename + ".service", Beschreibung);
                                    Action("/bin/sh", $"-c \"chmod +x {systemd_user}/{servicename}.service\"");
                                    break;
                                    case "enable":
                                    Action("/bin/sh", $"systemctl --user enable --now {servicename}.service\"");
                                    break;
                                    case "disable":
                                    Action("/bin/sh", $"-c \"systemctl --user disable --now {servicename}\"");
                                    break;
                                    case "start":
                                    Action("/bin/sh", $"-c \"systemctl --user start --now {servicename}\"");
                                    break;
                                    case "stop":
                                    Action("/bin/sh", $"-c \"systemctl --user stop --now {servicename}\"");
                                    break;
                                    case "restart":
                                    Action("/bin/sh", $"-c \"systemctl --user restart --now {servicename}\"");
                                    break;
                                    case "reload":
                                    Action("/bin/sh", $"-c \"systemctl --user daemon-reload\"");
                                    break; 
                                }
                                break;
                            }
                            case "Parameter":
                            {
                                var vorgang = (Parameter)element.vorgang;//((JObject)element.vorgang).ToObject<Parameter>();

                                var pfad = ($"{anwendungsordner}/{anwendung.Identifikation}/{paket_.Key}/{vorgang.Ordner}").Replace("//", "/");

                                var files = System.IO.Directory.GetFiles(pfad, "*.json", System.IO.SearchOption.AllDirectories);

                                foreach (var file in files)
                                {
                                    var file_content = System.IO.File.ReadAllText(file);
                                    foreach (var variable in vorgang.Variablen)
                                    {
                                        file_content = file_content.Replace(variable.Key, variable.Value);
                                    }
                                    System.IO.File.WriteAllText(file, file_content);
                                }
                                break;
                            }                                                    
                        }
                    }
                }
            }

            if (anwendung.Konfigurationselemente.ContainsKey(ressource_identifikation))
            {
                foreach (var element in anwendung.Konfigurationselemente[ressource_identifikation])
                {
                    switch (element.typ)
                    {
                        case "Datei": {
                            var vorgang = (Datei)element.vorgang;//((JObject)element.vorgang).ToObject<Datei>();
                            var datei_task = GetToFile(vorgang.Quelle + "/" + vorgang.Name, $"{anwendungsordner}/{anwendung.Identifikation}/{vorgang.Ordner}/{vorgang.Name}");
                            datei_task.Wait();
                            break;
                        }
                        case "Service": {
                            var vorgang = (Dienst)element.vorgang;//((JObject)element.vorgang).ToObject<Dienst>();
                            var servicename = $"{anwendung.Identifikation}.{vorgang.Name}";
                            var systemd_user = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";

                            switch (vorgang.Operation)
                            {
                                case "create":
                                var arbeitspfad = ($"{anwendungsordner}/{anwendung.Identifikation}/{vorgang.Arbeitspfad}").Replace("//", "/");
                                var pfad = ($"{anwendungsordner}/{anwendung.Identifikation}/{vorgang.Ziel}").Replace("//", "/");

                                string Beschreibung = $"[Unit]\n" +
                                $"Description={servicename}\n\n" +
                                "[Service]\n" +
                                $"WorkingDirectory={arbeitspfad}\n" +
                                $"ExecStart={pfad}\n" +
                                "Restart=always\n\n" +
                                "[Install]\n" +
                                "WantedBy=default.target";

                                System.IO.File.WriteAllText(systemd_user + "/" + servicename + ".service", Beschreibung);
                                Action("/bin/sh", $"-c \"chmod +x {systemd_user}/{servicename}.service\"");
                                break;
                                case "enable":
                                Action("/bin/sh", $"systemctl --user enable --now {servicename}.service\"");
                                break;
                                case "disable":
                                Action("/bin/sh", $"-c \"systemctl --user disable --now {servicename}\"");
                                break;
                                case "start":
                                Action("/bin/sh", $"-c \"systemctl --user start --now {servicename}\"");
                                break;
                                case "stop":
                                Action("/bin/sh", $"-c \"systemctl --user stop --now {servicename}\"");
                                break;
                                case "restart":
                                Action("/bin/sh", $"-c \"systemctl --user restart --now {servicename}\"");
                                break;
                                case "reload":
                                Action("/bin/sh", $"-c \"systemctl --user daemon-reload\"");
                                break; 
                            }
                            break;
                        }
                        case "Parameter":
                        {
                            var vorgang = (Parameter)element.vorgang;//((JObject)element.vorgang).ToObject<Parameter>();

                            var pfad = ($"{anwendungsordner}/{anwendung.Identifikation}/{vorgang.Ordner}").Replace("//", "/");

                            var files = System.IO.Directory.GetFiles(pfad, "*.json", System.IO.SearchOption.AllDirectories);

                            foreach (var file in files)
                            {
                                var file_content = System.IO.File.ReadAllText(file);
                                foreach (var variable in vorgang.Variablen)
                                {
                                    file_content = file_content.Replace(variable.Key, variable.Value);
                                }
                                System.IO.File.WriteAllText(file, file_content);
                            }
                            break;
                        }                                                    
                    }
                }
            }
        }

        static void AnwendungInstallieren(string anwendung, string instanz_identifikation, List<string> pseudonyme, string anwendungsordner)
        {
            var basis_objekt = JsonConvert.DeserializeObject<Basis>(System.IO.File.ReadAllText(anwendung + ".json"));

            AnwendungInstallieren(basis_objekt, instanz_identifikation, pseudonyme, anwendungsordner);
        }

        static void AnwendungInstallieren(Basis basis_objekt, string instanz_identifikation, List<string> pseudonyme, string anwendungsordner)
        {
            foreach (var pseudonym in pseudonyme)
            {
                if (!basis_objekt.Konfigurationspakete.ContainsKey(pseudonym)) continue;

                foreach (var paket_ in basis_objekt.Konfigurationspakete[pseudonym])
                {
                    var datei = GetToFile(Adresse + "/Konfigurationen/" + paket_.Value + ".json", "tmp/" + paket_.Key + ".json");
                    datei.Wait();
                    var paket = JsonConvert.DeserializeObject<Konfigurationspaket>(System.IO.File.ReadAllText("tmp/" + paket_.Key + ".json"));

                    foreach (var element in paket.Elemente)
                    {
                        switch (element.typ)
                        {
                            case "Datei": {
                                var vorgang = (Datei)element.vorgang;//((JObject)element.vorgang).ToObject<Datei>();
                                var datei_task = GetToFile(vorgang.Quelle + "/" + vorgang.Name, $"{anwendungsordner}/{instanz_identifikation}/{basis_objekt.Identifikation}/{paket_.Key}/{vorgang.Ordner}/{vorgang.Name}");
                                datei_task.Wait();
                                break;
                            }
                            case "Service": {
                                var vorgang = (Dienst)element.vorgang;//((JObject)element.vorgang).ToObject<Dienst>();
                                var servicename = $"{instanz_identifikation}.{basis_objekt}.{paket_.Key}.{vorgang.Name}";
                                var systemd_user = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";

                                switch (vorgang.Operation)
                                {
                                    case "create":
                                    var arbeitspfad = ($"{anwendungsordner}/{instanz_identifikation}/{basis_objekt}/{paket_.Key}/{vorgang.Arbeitspfad}").Replace("//", "/");
                                    var pfad = ($"{anwendungsordner}/{instanz_identifikation}/{basis_objekt}/{paket_.Key}/{vorgang.Ziel}").Replace("//", "/");

                                    string Beschreibung = $"[Unit]\n" +
                                    $"Description={servicename}\n\n" +
                                    "[Service]\n" +
                                    $"WorkingDirectory={arbeitspfad}\n" +
                                    $"ExecStart={pfad}\n" +
                                    "Restart=always\n\n" +
                                    "[Install]\n" +
                                    "WantedBy=default.target";

                                    System.IO.File.WriteAllText(systemd_user + "/" + servicename + ".service", Beschreibung);
                                    Action("/bin/sh", $"-c \"chmod +x {systemd_user}/{servicename}.service\"");
                                    break;
                                    case "enable":
                                    Action("/bin/sh", $"systemctl --user enable --now {servicename}.service\"");
                                    break;
                                    case "disable":
                                    Action("/bin/sh", $"-c \"systemctl --user disable --now {servicename}\"");
                                    break;
                                    case "start":
                                    Action("/bin/sh", $"-c \"systemctl --user start --now {servicename}\"");
                                    break;
                                    case "stop":
                                    Action("/bin/sh", $"-c \"systemctl --user stop --now {servicename}\"");
                                    break;
                                    case "restart":
                                    Action("/bin/sh", $"-c \"systemctl --user restart --now {servicename}\"");
                                    break;
                                    case "reload":
                                    Action("/bin/sh", $"-c \"systemctl --user daemon-reload\"");
                                    break; 
                                }
                                break;
                            }
                            case "Parameter":
                            {
                                var vorgang = (Parameter)element.vorgang;//((JObject)element.vorgang).ToObject<Parameter>();

                                var pfad = ($"{anwendungsordner}/{instanz_identifikation}/{basis_objekt}/{paket_.Key}/{vorgang.Ordner}").Replace("//", "/");

                                var files = System.IO.Directory.GetFiles(pfad, "*.json", System.IO.SearchOption.AllDirectories);

                                foreach (var file in files)
                                {
                                    var file_content = System.IO.File.ReadAllText(file);
                                    foreach (var variable in vorgang.Variablen)
                                    {
                                        file_content = file_content.Replace(variable.Key, variable.Value);
                                    }
                                    System.IO.File.WriteAllText(file, file_content);
                                }
                                break;
                            }                                                    
                        }
                    }
                }
            }

            foreach (var pseudonym in pseudonyme)
            {
                if (basis_objekt.Konfigurationselemente.ContainsKey(pseudonym))
                {
                    foreach (var element in basis_objekt.Konfigurationselemente[pseudonym])
                    {
                        switch (element.typ)
                        {
                            case "Datei": {
                                var vorgang = (Datei)element.vorgang;//((JObject)element.vorgang).ToObject<Datei>();
                                var datei_task = GetToFile(vorgang.Quelle + "/" + vorgang.Name, $"{anwendungsordner}/{basis_objekt.Identifikation}/{vorgang.Ordner}/{vorgang.Name}");
                                datei_task.Wait();
                                break;
                            }
                            case "Service": {
                                var vorgang = (Dienst)element.vorgang;//((JObject)element.vorgang).ToObject<Dienst>();
                                var servicename = $"{basis_objekt.Identifikation}.{vorgang.Name}";
                                var systemd_user = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";

                                switch (vorgang.Operation)
                                {
                                    case "create":
                                    var arbeitspfad = ($"{anwendungsordner}/{basis_objekt.Identifikation}/{vorgang.Arbeitspfad}").Replace("//", "/");
                                    var pfad = ($"{anwendungsordner}/{basis_objekt.Identifikation}/{vorgang.Ziel}").Replace("//", "/");

                                    string Beschreibung = $"[Unit]\n" +
                                    $"Description={servicename}\n\n" +
                                    "[Service]\n" +
                                    $"WorkingDirectory={arbeitspfad}\n" +
                                    $"ExecStart={pfad}\n" +
                                    "Restart=always\n\n" +
                                    "[Install]\n" +
                                    "WantedBy=default.target";

                                    System.IO.File.WriteAllText(systemd_user + "/" + servicename + ".service", Beschreibung);
                                    Action("/bin/sh", $"-c \"chmod +x {systemd_user}/{servicename}.service\"");
                                    break;
                                    case "enable":
                                    Action("/bin/sh", $"systemctl --user enable --now {servicename}.service\"");
                                    break;
                                    case "disable":
                                    Action("/bin/sh", $"-c \"systemctl --user disable --now {servicename}\"");
                                    break;
                                    case "start":
                                    Action("/bin/sh", $"-c \"systemctl --user start --now {servicename}\"");
                                    break;
                                    case "stop":
                                    Action("/bin/sh", $"-c \"systemctl --user stop --now {servicename}\"");
                                    break;
                                    case "restart":
                                    Action("/bin/sh", $"-c \"systemctl --user restart --now {servicename}\"");
                                    break;
                                    case "reload":
                                    Action("/bin/sh", $"-c \"systemctl --user daemon-reload\"");
                                    break; 
                                }
                                break;
                            }
                            case "Parameter":
                            {
                                var vorgang = (Parameter)element.vorgang;//((JObject)element.vorgang).ToObject<Parameter>();

                                var pfad = ($"{anwendungsordner}/{basis_objekt.Identifikation}/{vorgang.Ordner}").Replace("//", "/");

                                var files = System.IO.Directory.GetFiles(pfad, "*.json", System.IO.SearchOption.AllDirectories);

                                foreach (var file in files)
                                {
                                    var file_content = System.IO.File.ReadAllText(file);
                                    foreach (var variable in vorgang.Variablen)
                                    {
                                        file_content = file_content.Replace(variable.Key, variable.Value);
                                    }
                                    System.IO.File.WriteAllText(file, file_content);
                                }
                                break;
                            }                                                    
                        }
                    }
                }
            }
        }

        static string Adresse;        

        static void Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");
            var Automatisierungsressource = new isci.Beschreibung.Automatisierungsressource()
            {
                Identifikation = konfiguration.AUTOMATISIERUNG_RESSOURCE,
                Handler = konfiguration.IdentifikationHandler,
                Beschreibung = "Automatisierungsressource " + System.Environment.MachineName,
                Name = System.Environment.MachineName
            };
            var Handler = new isci.Beschreibung.Handler()
            {
                Identifikation = konfiguration.IdentifikationHandler,
                Automatisierungsressourcen = new List<string>(){konfiguration.AUTOMATISIERUNG_RESSOURCE},
                Beschreibung = "Handler " + System.Environment.MachineName,
                Name = "Handler_" + System.Environment.MachineName
            };

            Adresse = konfiguration.Plattform;
            var systemd_user = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";

            while(true)
            {
                var Ist = new List<string>();
                var Soll = new List<string>();

                var Installieren = new List<string>();
                var Aktualisieren = new List<string>();
                var Entfernen = new List<string>();

                try
                {
                    Ist = System.IO.File.ReadAllLines("Ist.json").ToList<string>();
                } catch {

                }

                try
                {
                    var get = GetToFile(Adresse + "/Anweisungen/" + konfiguration.AUTOMATISIERUNG_RESSOURCE + ".json", konfiguration.AUTOMATISIERUNG_RESSOURCE + ".json");
                    get.Wait();
                    Soll = System.IO.File.ReadAllLines(konfiguration.AUTOMATISIERUNG_RESSOURCE + ".json").ToList<string>();
                } catch {

                }

                foreach (var anwendung in Soll)
                {
                    if (System.IO.File.Exists(anwendung + ".json") && Ist.Contains(anwendung))
                    {
                        var get = GetToFile(Adresse + "/Anwendungen/" + anwendung + ".json", anwendung + "_tmp.json");
                        get.Wait();
                        var anwendung_tmp_json = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(anwendung + "_tmp.json"));
                        var anwendung_json = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(anwendung + ".json"));

                        if (anwendung_tmp_json.Equals(anwendung_json))
                        {
                            System.IO.File.Delete(anwendung + "_tmp.json");
                        } else {
                            Aktualisieren.Add(anwendung);
                        }
                    } else {
                        var get = GetToFile(Adresse + "/Anwendungen/" + anwendung + ".json", anwendung + ".json");
                        get.Wait();
                        Installieren.Add(anwendung);
                    }
                }

                foreach (var anwendung in Ist)
                {
                    if (!Soll.Contains(anwendung))
                    {
                        Entfernen.Add(anwendung);
                    }
                }

                foreach (var anwendung in Entfernen)
                {
                    var services = System.IO.Directory.GetFiles(systemd_user, $"{anwendung}.*");
                    foreach (var service in services)
                    {
                        Action("/bin/sh", $"-c \"systemctl --user disable --now {service}\"");
                        Action("/bin/sh", $"-c \"systemctl --user kill --now {service}\"");
                        System.IO.File.Delete(service);
                    }
                    System.IO.Directory.Delete(anwendung, true);
                    System.IO.File.Delete(anwendung + ".json");
                    Ist.Remove(anwendung);
                }
                Entfernen.Clear();

                foreach (var anwendung in Aktualisieren)
                {
                    var services = System.IO.Directory.GetFiles(systemd_user, $"{anwendung}.*");
                    foreach (var service in services)
                    {
                        Action("/bin/sh", $"-c \"systemctl --user disable --now {service}\"");
                        Action("/bin/sh", $"-c \"systemctl --user kill --now {service}\"");
                        System.IO.File.Delete(service);
                    }
                    System.IO.Directory.Delete(anwendung, true);
                    System.IO.File.Move(anwendung + "_tmp.json", anwendung + ".json", true);

                    AnwendungInstallieren(anwendung + ".json", konfiguration.AUTOMATISIERUNG_ANWENDUNGEN, konfiguration.AUTOMATISIERUNG_RESSOURCE);
                    Ist.Add(anwendung);
                }
                Aktualisieren.Clear();

                foreach (var anwendung in Installieren)
                {
                    AnwendungInstallieren(anwendung + ".json", konfiguration.AUTOMATISIERUNG_ANWENDUNGEN, konfiguration.AUTOMATISIERUNG_RESSOURCE);
                    Ist.Add(anwendung);
                }
                Installieren.Clear();

                System.IO.File.WriteAllText("Ist.json", Newtonsoft.Json.JsonConvert.SerializeObject(Ist));
                System.Threading.Thread.Sleep(60000);
            }         
        }
    }
}
