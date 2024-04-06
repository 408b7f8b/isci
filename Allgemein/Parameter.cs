using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;

namespace isci.Allgemein
{
    public class Parameter
    {
        [AttributeUsage(AttributeTargets.Field)]
        public class IgnoreParse : Attribute
        {
            public IgnoreParse(){}
        }

        public class fromArgs : Attribute
        {
            public fromArgs(){}
        }

        public class fromEnv : Attribute
        {
            public fromEnv(){}
        }

        static void NullPruefen(object var, string Name, bool auchKonfigurationsdatei)
        {
            if (var == null) 
            {
                var msg = ( auchKonfigurationsdatei ?
                "'" + Name + "' undefiniert, Umgebungsvariable ('ISCI_${" + Name.ToUpper() + "}) anlegen oder als '" + Name + "' als Startparameter oder über die Konfigurationsdatei vorgeben." :
                "'" + Name + "' undefiniert, Umgebungsvariable ('ISCI_${" + Name.ToUpper() + "}) anlegen oder als '" + Name + "' als Startparameter vorgeben.");

                Logger.Fatal(msg);
                System.Environment.Exit(-1);
            }
        }

        static void LeerPruefen(string var, string Name, bool auchKonfigurationsdatei)
        {
            if (var.Trim() == "")
            {
                var msg = ( auchKonfigurationsdatei ?
                "'" + Name + "' leer, Umgebungsvariable ('ISCI_${" + Name.ToUpper() + "}) anlegen oder als '" + Name + "' als Startparameter oder über die Konfigurationsdatei vorgeben." :
                "'" + Name + "' leer, Umgebungsvariable ('ISCI_${" + Name.ToUpper() + "}) anlegen oder als '" + Name + "' als Startparameter vorgeben.");

                Logger.Fatal(msg);
                System.Environment.Exit(-1);
            }
        }

        [fromArgs, fromEnv]
        public string Ressource = "";
        [fromArgs, fromEnv]
        public string Identifikation = "";
        [fromArgs, fromEnv]
        public string OrdnerAnwendungen = "";
        [fromArgs, fromEnv]
        public string OrdnerDatenstrukturen = "";
        [fromArgs, fromEnv]
        public string Anwendung = "";
        [fromArgs, fromEnv]
        public uint PauseArbeitsschleifeUs = 100;
        [fromArgs, fromEnv]
        public bool LoggingInKonsoleAktiv = false;
        [fromArgs, fromEnv]
        public Logger.Stufe LoggingInKonsoleMindeststufe = Logger.Stufe.INFORMATION;
        [fromArgs, fromEnv]
        public bool LoggingInDateiAktiv = true;
        [fromArgs, fromEnv]        
        public Logger.Stufe LoggingInDateiMindeststufe = Logger.Stufe.INFORMATION;
        [fromArgs, fromEnv]
        public uint LoggingInDateiMaxDateiGroesseInMb = 1;

        [IgnoreParse]
        public string OrdnerDatenmodelle;
        [IgnoreParse]
        public string OrdnerEreignismodelle;
        [IgnoreParse]
        public string OrdnerFunktionsmodelle;
        [IgnoreParse]
        public string OrdnerSchnittstellen;
        [IgnoreParse]
        public string OrdnerBeschreibungen;
        [IgnoreParse]
        public string OrdnerLogs;
        [IgnoreParse]
        public string OrdnerKonfigurationen;

        public void Beschreiben()
        {
            var Parameterbeschreibung = new List<string>();
            var felder = GetType().GetFields();
            foreach (var f in felder)
            {
                var ParameterbeschreibungEintrag = f.Name + ": " + f.FieldType.FullName;
                Parameterbeschreibung.Add(ParameterbeschreibungEintrag);
            }

            System.IO.File.WriteAllLines("MoeglicheParameter.meta", Parameterbeschreibung.ToArray());
        }

        public Parameter(string[] args)
        {
            Helfer.SetzeArchitektur();

            Beschreiben();

            var param_env = new System.Collections.Generic.Dictionary<string, string>();
            var envVariablen = Environment.GetEnvironmentVariables();

            foreach (System.Collections.DictionaryEntry variable in envVariablen)
            {
                var envName = (string)variable.Key;
                var wert = (string)variable.Value;
                if (envName.StartsWith("ISCI_"))
                {
                    var value = wert;
                    var name = envName.Substring(5);
                    param_env[name] = value;
                    Logger.Information($"Umgebungsvariable {envName} als {name}={value} hinzugefügt");
                }
            }

            var param_args = new System.Collections.Generic.Dictionary<string, string>();
            
            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    param_args[args[i]] = args[i + 1];
                    Logger.Information($"Startargument {args[i]} als {args[i]}={args[i + 1]} hinzugefügt");
                }
                else
                {
                    param_args[args[i]] = null;
                    Logger.Warnung($"Startargument {args[i]} als {args[i]}=null hinzugefügt");
                }
            }

            Logger.Debug("Versuche initiale Konfiguration aus Umgebungsvariablen und Startargumenten zu befüllen. Umgebungsvariablen werden bevorzugt.");
            var felder = GetType().GetFields();
            foreach (var f in felder)
            {
                if (f.FieldType != typeof(string)) continue;

                Logger.Debug($"Versuche {f.Name} zu befüllen.");

                string wert;
                if (param_env.ContainsKey(f.Name) && Attribute.IsDefined(f, attributeType: typeof(fromEnv)))
                {
                    wert = param_env[f.Name];
                    Logger.Debug($"{f.Name} ist definiert in Umgebungsvariable: {wert}");
                } else if (param_args.ContainsKey(f.Name) && Attribute.IsDefined(f, attributeType: typeof(fromArgs)))
                {
                    wert = param_args[f.Name];
                    Logger.Debug($"{f.Name} ist definiert über Startargument: {wert}");                    
                } else {
                    Logger.Debug($"{f.Name} ist nicht initial definiert.");
                    continue;
                }

                Logger.Information($"Konfigurationsparameter aus Startargument oder Umgebungsvariable: {f.Name}={wert}");
                this.GetType().GetField(f.Name).SetValue(this, wert);
            }
            Logger.Debug("Ende Versuch initiale Konfiguration aus Umgebungsvariablen und Startargumenten zu befüllen.");


            var konfigurationsdatei = "";
            if (Identifikation != null && Identifikation != "")
            {
                if (OrdnerAnwendungen != null && OrdnerAnwendungen != "" && Anwendung == null && Anwendung == "")
                {
                    OrdnerKonfigurationen = (OrdnerAnwendungen + "/" + Anwendung + "/Konfigurationen").Replace("//", "/");
                    konfigurationsdatei = OrdnerKonfigurationen + "/" + Identifikation + ".json";
                } else {
                    konfigurationsdatei = Identifikation + ".json";
                }
            }

            if (!System.IO.File.Exists(konfigurationsdatei))
            {
                konfigurationsdatei = "konfiguration.json";
                Logger.Information("Keine Konfigurationsdatei über Konfigurationsordner, versuche lokale Konfigurationsdatei konfiguration.json");
            }

            var param_file = new Newtonsoft.Json.Linq.JObject();            

            if (System.IO.File.Exists(konfigurationsdatei))
            {
                Logger.Information("Konfigurationsdatei: " + konfigurationsdatei);
                try {
                    param_file = Newtonsoft.Json.Linq.JObject.Parse(System.IO.File.ReadAllText(konfigurationsdatei));
                } catch (System.Exception e){
                    Logger.Fatal($"Ausnahme beim Verarbeiten der Konfigurationsdatei: {e.Message}");
                    System.Environment.Exit(-1);
                }
            } else {
                Logger.Warnung("Konfigurationsdatei " + konfigurationsdatei + " existiert nicht.");
            }

            Logger.Debug("Versuche gesamte Konfiguration aus Umgebungsvariablen, Startargumenten und Konfigurationsdatei zu befüllen. Konfigurationsdatei > Startargument > Umgebungsvariable.");
            var Parameterbeschreibung = new List<string>();
            foreach (var f in felder)
            {
                var in_env_vorhanden = param_env.ContainsKey(f.Name);
                var in_args_vorhanden = param_args.ContainsKey(f.Name);
                var in_datei_vorhanden = param_file[f.Name] != null;

                Logger.Debug($"Konfigurationsparameter {f.Name} in Umgebungsvariablen {(in_env_vorhanden ? "vorhanden" : "nicht vorhanden")}.");
                Logger.Debug($"Konfigurationsparameter {f.Name} in Umgebungsvariablen {(in_args_vorhanden ? "vorhanden" : "nicht vorhanden")}.");
                Logger.Debug($"Konfigurationsparameter {f.Name} in Umgebungsvariablen {(in_datei_vorhanden ? "vorhanden" : "nicht vorhanden")}.");

                try
                {
                    Logger.Debug($"Suche Konfigurationsparameter {f.Name}");
                    var feld = GetType().GetField(f.Name);
                    var feldtyp = feld.FieldType;
                    Logger.Debug($"Erwarteter Typ für Konfigurationsparameter: " + feldtyp.Name);

                    if (feldtyp == typeof(string))
                    {
                        if (in_env_vorhanden || in_args_vorhanden) {
                            //bereits zuvor verarbeitet
                        } else if (in_datei_vorhanden)
                        {
                            var o = param_file.SelectToken(f.Name).ToObject<string>();
                            feld.SetValue(this, o);
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                    }
                    else if (feldtyp == typeof(int))
                    {
                        int o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<int>();
                        } else if (in_env_vorhanden) {
                            o = int.Parse(param_env[f.Name]);
                        } else if (in_args_vorhanden) {
                            o = int.Parse(param_args[f.Name]);
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                        feld.SetValue(this, o);
                    }
                    else if (feldtyp == typeof(uint))
                    {
                        uint o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<uint>();
                        } else if (in_env_vorhanden) {
                            o = uint.Parse(param_env[f.Name]);
                        } else if (in_args_vorhanden) {
                            o = uint.Parse(param_args[f.Name]);
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                        feld.SetValue(this, o);
                    }
                    else if (feldtyp == typeof(bool))
                    {
                        bool o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<bool>();
                        } else if (in_env_vorhanden) {
                            o = bool.Parse(param_env[f.Name]);
                        } else if (in_args_vorhanden) {
                            o = bool.Parse(param_args[f.Name]);
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                        feld.SetValue(this, o);
                    }
                    else if (feldtyp == typeof(double))
                    {
                        double o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<double>();
                        } else if (in_env_vorhanden) {
                            o = double.Parse(param_env[f.Name]);
                        } else if (in_args_vorhanden) {
                            o = double.Parse(param_args[f.Name]);
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                        feld.SetValue(this, o);
                    }
                    else if (feldtyp == typeof(Int32[]))
                    {
                        Int32[] o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<Int32[]>();
                        } else if (in_env_vorhanden) {
                            var als_token = Newtonsoft.Json.Linq.JArray.Parse(param_env[f.Name]);
                            o = als_token.Select(jt => (int)jt).ToArray();
                        } else if (in_args_vorhanden) {
                            var als_token = Newtonsoft.Json.Linq.JArray.Parse(param_args[f.Name]);
                            o = als_token.Select(jt => (int)jt).ToArray();
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                        feld.SetValue(this, o);
                    }
                    else if (feldtyp == typeof(string[]))
                    {
                        string[] o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<string[]>();
                        } else if (in_env_vorhanden) {
                            var als_token = Newtonsoft.Json.Linq.JArray.Parse(param_env[f.Name]);
                            o = als_token.Select(jt => (string)jt).ToArray();
                        } else if (in_args_vorhanden) {
                            var als_token = Newtonsoft.Json.Linq.JArray.Parse(param_args[f.Name]);
                            o = als_token.Select(jt => (string)jt).ToArray();
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                        feld.SetValue(this, o);
                    }
                    else if (feldtyp == typeof(Logger.Stufe))
                    {
                        Logger.Stufe o;
                        if (in_datei_vorhanden) {
                            o = param_file.SelectToken(f.Name).ToObject<Logger.Stufe>();
                        } else if (in_env_vorhanden) {
                            if (!Enum.TryParse<Logger.Stufe>(param_args[f.Name], out o) )
                            {
                                Logger.Fehler("Fehler bei der Verarbeitung der Loggingstufe bei Konfigurationsparameter " + f.Name);
                                continue;
                            }
                            var als_token = Enum.Parse(typeof(Logger.Stufe), param_args[f.Name]);
                        } else if (in_args_vorhanden) {
                            if (!Enum.TryParse<Logger.Stufe>(param_args[f.Name], out o) )
                            {
                                Logger.Fehler("Fehler bei der Verarbeitung der Loggingstufe bei Konfigurationsparameter " + f.Name);
                                continue;
                            }
                            var als_token = Enum.Parse(typeof(Logger.Stufe), param_args[f.Name]);
                        } else {
                            Logger.Warnung("Konfigurationsparameter " + f.Name + " ist weder durch Startargument, Umgebungsvariablen noch Konfigurationsdatei definiert.");
                            continue;
                        }
                    }

                    Logger.Information("Konfigurationsparameter " + feld.Name + "=" + feld.GetValue(this).ToString());
                } catch (System.Exception e) {
                    Logger.Fehler($"Ausnahme bei der Verarbeitung des Konfigurationsparameters {f.Name}: {e.Message}");
                }
            }

            Logger.Debug("Prüfe kritische Konfigurationsparameter.");

            NullPruefen(OrdnerAnwendungen, "OrdnerAnwendungen", false);
            NullPruefen(OrdnerDatenstrukturen, "OrdnerDatenstrukturen", false);
            NullPruefen(Ressource, "Ressource", true);
            NullPruefen(Identifikation, "Identifikation", true);            
            NullPruefen(Anwendung, "Anwendung", true);
            LeerPruefen(OrdnerAnwendungen, "OrdnerAnwendungen", false);
            LeerPruefen(OrdnerDatenstrukturen, "OrdnerDatenstrukturen", false);
            LeerPruefen(Ressource, "Ressource", true);
            LeerPruefen(Identifikation, "Identifikation", true);
            LeerPruefen(Anwendung, "Anwendung", true);

            OrdnerDatenmodelle = (OrdnerAnwendungen + "/" + Anwendung + "/Datenmodelle").Replace("//", "/");
            OrdnerEreignismodelle = (OrdnerAnwendungen + "/" + Anwendung + "/Ereignismodelle").Replace("//", "/");
            OrdnerFunktionsmodelle = (OrdnerAnwendungen + "/" + Anwendung + "/Funktionsmodelle").Replace("//", "/");
            OrdnerSchnittstellen = (OrdnerAnwendungen + "/" + Anwendung + "/Schnittstellen").Replace("//", "/");
            OrdnerBeschreibungen = (OrdnerAnwendungen + "/" + Anwendung + "/Beschreibungen").Replace("//", "/");
            OrdnerLogs = (OrdnerAnwendungen + "/" + Anwendung + "/Logs").Replace("//", "/");
            OrdnerKonfigurationen = (OrdnerAnwendungen + "/" + Anwendung + "/Konfigurationen").Replace("//", "/");

            Logger.Information("Richte Logging anhand Konfiguration ein.");
            Logger.Konfigurieren(this);

            Logger.Debug("Prüfe und erstelle nach Notwendigkeit Ordner.");
            Helfer.OrdnerPruefenErstellen(OrdnerDatenmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerEreignismodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerFunktionsmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerSchnittstellen);
            Helfer.OrdnerPruefenErstellen(OrdnerBeschreibungen);
            Helfer.OrdnerPruefenErstellen(OrdnerLogs);
            Helfer.OrdnerPruefenErstellen(OrdnerKonfigurationen);
        }
    }
}