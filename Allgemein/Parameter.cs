using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace isci.Allgemein
{
    public class Parameter
    {
        [AttributeUsage(AttributeTargets.Field)]
        class IgnoreParse : Attribute
        {
            public IgnoreParse(){}
        }

        class fromArgs : Attribute
        {
            public fromArgs(){}
        }

        class fromEnv : Attribute
        {
            public fromEnv(){}
        }
/*
        static Parameter ausDatei(string datei)
        {
            var ret = new Parameter();

            Identifikation = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_IDENTIFIKATION");

            var tmp = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_RESSOURCE");
            if (tmp == null) Ressource = System.Environment.MachineName;
            else Ressource = tmp;

            Anwendung = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNG");

            OrdnerAnwendung = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN");
            OrdnerDatenstruktur = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_DATENSTRUKTUREN");
            OrdnerDatenmodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN");
            OrdnerEreignismodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN");
            OrdnerFunktionsmodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN");
            OrdnerSchnittstellen = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN");
            OrdnerBeschreibungen = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN");

            if (!System.IO.File.Exists(datei))
            {
                System.Console.WriteLine("Konfigurationsdatei " + datei + " existiert nicht. Erstelle Beispieldatei, falls noch nicht vorhanden.");
                if (!System.IO.File.Exists("konfiguration_beispiel.json")) System.IO.File.WriteAllText("konfiguration_beispiel.json", Newtonsoft.Json.JsonConvert.SerializeObject(this));
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
                            else if (feldtyp == typeof(Ausführungstransition[]))
                            {
                                var o = t.SelectToken(f.Name).ToObject<Ausführungstransition[]>();
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
            
            NullPruefen(Identifikation, "Identifikation");
            NullPruefen(Anwendung, "Anwendung");
            NullPruefen(OrdnerAnwendung, "OrdnerAnwendung");
            NullPruefen(OrdnerDatenstruktur, "OrdnerDatenstruktur");
            NullPruefen(OrdnerDatenmodelle, "OrdnerDatenmodelle");
            NullPruefen(OrdnerEreignismodelle, "OrdnerEreignismodelle");
            NullPruefen(OrdnerFunktionsmodelle, "OrdnerFunktionsmodelle");
            NullPruefen(OrdnerSchnittstellen, "OrdnerSchnittstellen");
            NullPruefen(OrdnerBeschreibungen, "OrdnerBeschreibungen");

            if (!OrdnerAnwendung.EndsWith(Anwendung)) OrdnerAnwendung = (OrdnerAnwendung + "/" + Anwendung).Replace("//", "/");
            if (!OrdnerDatenstruktur.EndsWith(Anwendung)) OrdnerDatenstruktur = (OrdnerDatenstruktur + "/" + Anwendung).Replace("//", "/");
            if (!OrdnerDatenmodelle.EndsWith(Anwendung + "/Datenmodelle")) OrdnerDatenmodelle = (OrdnerDatenmodelle + "/" + Anwendung + "/Datenmodelle").Replace("//", "/");
            if (!OrdnerEreignismodelle.EndsWith(Anwendung + "/Ereignismodelle")) OrdnerEreignismodelle = (OrdnerEreignismodelle + "/" + Anwendung + "/Ereignismodelle").Replace("//", "/");
            if (!OrdnerFunktionsmodelle.EndsWith(Anwendung + "/Funktionsmodelle")) OrdnerFunktionsmodelle = (OrdnerFunktionsmodelle + "/" + Anwendung + "/Funktionsmodelle").Replace("//", "/");
            if (!OrdnerSchnittstellen.EndsWith(Anwendung + "/Schnittstellen")) OrdnerSchnittstellen = (OrdnerSchnittstellen + "/" + Anwendung + "/Schnittstellen").Replace("//", "/");
            if (!OrdnerBeschreibungen.EndsWith(Anwendung + "/Beschreibungen")) OrdnerBeschreibungen = (OrdnerBeschreibungen + "/" + Anwendung + "/Beschreibungen").Replace("//", "/");

            Helfer.OrdnerPruefenErstellen(OrdnerAnwendung);
            Helfer.OrdnerPruefenErstellen(OrdnerDatenstruktur);
            Helfer.OrdnerPruefenErstellen(OrdnerDatenmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerEreignismodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerFunktionsmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerSchnittstellen);
            Helfer.OrdnerPruefenErstellen(OrdnerBeschreibungen);


        }*/

        static void NullPruefen(object var, string Name)
        {
            if (var == null) 
            {
                Logger.Loggen(Logger.Qualität.ERROR, "'" + Name + "' undefiniert, Umgebungsvariable ('ISCI_${" + Name.ToUpper() + "}) anlegen oder als '" + Name + "' als Startparameter oder über die Konfigurationsdatei vorgeben.");
                System.Environment.Exit(-1);
            }
        }

        static void LeerPruefen(string var, string Name)
        {
            if (var.Trim() == "")
            {
                Logger.Loggen(Logger.Qualität.ERROR, "'" + Name + "' leer, Umgebungsvariable ('ISCI_${" + Name.ToUpper() + "}) anlegen oder als '" + Name + "' als Startparameter oder über die Konfigurationsdatei vorgeben.");
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

        public Parameter(string[] args)
        {
            var param_env = new System.Collections.Generic.Dictionary<string, string>();

            var envVariablen = Environment.GetEnvironmentVariables();

            foreach (System.Collections.DictionaryEntry variable in envVariablen)
            {
                var name = (string)variable.Key;
                var wert = (string)variable.Value;
                if (name.StartsWith("ISCI_"))
                {
                    var value = wert;
                    param_env[name.Substring(5)] = value;
                }
            }

            var param_args = new System.Collections.Generic.Dictionary<string, string>();
            
            for (int i = 0; i < args.Length; i += 2)
            {
                if (i + 1 < args.Length)
                {
                    param_args[args[i]] = args[i + 1];
                }
                else
                {
                    param_args[args[i]] = null;
                }
            }

            var felder = GetType().GetFields();
            foreach (var f in felder)
            {
                if (param_env.ContainsKey(f.Name) && Attribute.IsDefined(f, attributeType: typeof(fromEnv))) this.GetType().GetField(f.Name).SetValue(this, param_env[key: f.Name]);
                else if (param_args.ContainsKey(f.Name) && Attribute.IsDefined(f, attributeType: typeof(fromArgs))) this.GetType().GetField(f.Name).SetValue(this, param_args[f.Name]);
            }

            var konfigurationsdatei = (OrdnerAnwendungen + "/" + Anwendung + "/Konfigurationen/" + Identifikation + ".json").Replace("//", "/");
            if (System.IO.File.Exists(konfigurationsdatei))
            {
                try
                {
                    var t = Newtonsoft.Json.Linq.JToken.Parse(System.IO.File.ReadAllText(konfigurationsdatei));
                    Logger.Loggen(Logger.Qualität.INFO, "Konfigdatei: " + konfigurationsdatei);

                    foreach (var f in felder)
                    {
                        if (Attribute.IsDefined(f, attributeType: typeof(IgnoreParse))) continue;

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
                            else if (feldtyp == typeof(Ausführungstransition[]))
                            {
                                var o = t.SelectToken(f.Name).ToObject<Ausführungstransition[]>();
                                feld.SetValue(this, o);
                            } else {
                                var o = t.SelectToken(f.Name).ToObject<object>();
                                feld.SetValue(this, o);
                            }

                            try {
                                Logger.Loggen(Logger.Qualität.INFO, "Konfigparam " + feld.Name + ": " + feld.GetValue(this).ToString());
                            } catch (System.Exception e) {
                                Logger.Loggen(Logger.Qualität.ERROR, "Fehler beim Lesen und Setzen des Konfigurationsparameters " + feld.Name + ": " + e.Message);
                            }
                        }
                        catch { }
                    }
                } catch { }
            } else {
                Logger.Loggen(Logger.Qualität.INFO, "Konfigdatei: " + konfigurationsdatei + " existiert nicht.");
            }

            NullPruefen(Ressource, "Ressource");
            NullPruefen(Identifikation, "Identifikation");
            NullPruefen(OrdnerAnwendungen, "OrdnerAnwendungen");
            NullPruefen(OrdnerDatenstrukturen, "OrdnerDatenstrukturen");
            NullPruefen(Anwendung, "Anwendung");
            LeerPruefen(Ressource, "Ressource");
            LeerPruefen(Identifikation, "Identifikation");
            LeerPruefen(OrdnerAnwendungen, "OrdnerAnwendungen");
            LeerPruefen(OrdnerDatenstrukturen, "OrdnerDatenstrukturen");
            LeerPruefen(Anwendung, "Anwendung");

            Logger.Konfigurieren(this);

            OrdnerDatenmodelle = (OrdnerAnwendungen + "/" + Anwendung + "/Datenmodelle").Replace("//", "/");
            OrdnerEreignismodelle = (OrdnerAnwendungen + "/" + Anwendung + "/Ereignismodelle").Replace("//", "/");
            OrdnerFunktionsmodelle = (OrdnerAnwendungen + "/" + Anwendung + "/Funktionsmodelle").Replace("//", "/");
            OrdnerSchnittstellen = (OrdnerAnwendungen + "/" + Anwendung + "/Schnittstellen").Replace("//", "/");
            OrdnerBeschreibungen = (OrdnerAnwendungen + "/" + Anwendung + "/Beschreibungen").Replace("//", "/");

            Helfer.OrdnerPruefenErstellen(OrdnerDatenmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerEreignismodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerFunktionsmodelle);
            Helfer.OrdnerPruefenErstellen(OrdnerSchnittstellen);
            Helfer.OrdnerPruefenErstellen(OrdnerBeschreibungen);
        }
        /*public Parameter()
        {
            var felder = GetType().GetFields();

            foreach (var f in felder)
            {
                try
                {
                    string var_string = "";
                    var feld = GetType().GetField(f.Name);
                    var feldtyp = feld.FieldType;

                    try {
                        var_string = Environment.GetEnvironmentVariable(f.Name);
                    } catch {
                        Environment.SetEnvironmentVariable(f.Name, feld.GetValue(this).ToString());
                        continue;
                    }

                    if (feldtyp == typeof(string))
                    {
                        feld.SetValue(this, var_string);
                    }
                    else if (feldtyp == typeof(int))
                    {
                        feld.SetValue(this, Int32.Parse(var_string));
                    }
                    else if (feldtyp == typeof(uint))
                    {
                        feld.SetValue(this, UInt32.Parse(var_string));
                    }
                    else if (feldtyp == typeof(bool))
                    {
                        feld.SetValue(this, Boolean.Parse(var_string));
                    }
                    else if (feldtyp == typeof(double))
                    {
                        feld.SetValue(this, Double.Parse(var_string));
                    } else {
                        if (!var_string.StartsWith("{")) var_string = "{" + var_string;
                        if (!var_string.EndsWith("}")) var_string += "}";

                        if (feldtyp == typeof(Int32[]))
                        {
                            feld.SetValue(this, Newtonsoft.Json.JsonToken.Parse(typeof(Int32[]), var_string));
                        }
                        else if (feldtyp == typeof(string[]))
                        {
                            feld.SetValue(this, Newtonsoft.Json.JsonToken.Parse(typeof(string[]), var_string));
                        }
                        else if (feldtyp == typeof(Ausführungstransition[]))
                        {
                            feld.SetValue(this, Newtonsoft.Json.JsonToken.Parse(typeof(Ausführungstransition[]), var_string));
                        }
                    }

                    try {
                        Console.WriteLine("Konfigparam " + feld.Name + ": " + feld.GetValue(this).ToString());
                    } catch { }
                }
                catch { }
            }
        }*/
    }
}