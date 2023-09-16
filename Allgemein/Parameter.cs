using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Parameter
    {
        static void NullPruefen(object var, string Name)
        {
            if (var == null) 
            {
                System.Console.WriteLine(Name + " undefiniert, setze Umgebungsvariable oder in konfiguration.json"); System.Environment.Exit(-1);
            }
        }

        public string Ressource;
        public string Identifikation;
        public string OrdnerAnwendung;
        public string OrdnerDatenstruktur;
        public string Anwendung;
        public string OrdnerDatenmodelle;
        public string OrdnerEreignismodelle;
        public string OrdnerFunktionsmodelle;
        public string OrdnerSchnittstellen;
        public string OrdnerBeschreibungen;
        public Ausführungstransition[] Ausführungstransitionen;

        public Parameter(string datei)
        {
            var tmp = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_IDENTIFIKATION");
            if (tmp == null) Identifikation = tmp;

            tmp = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_RESSOURCE");
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
                System.Console.WriteLine("Konfigurationsdatei " + datei + "existiert nicht. Erstelle Beispieldatei.");
                System.IO.File.WriteAllText("konfiguration_beispiel.json", Newtonsoft.Json.JsonConvert.SerializeObject(this));
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
        }

        public Parameter()
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
        }
    }
}