using System;
using System.Linq;

namespace isci.Allgemein
{
    public class Parameter
    {
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
            Ressource = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_RESSOURCE");

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

            OrdnerAnwendung = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung;
            OrdnerDatenstruktur = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_DATENSTRUKTUREN") + "/" + Anwendung;
            OrdnerDatenmodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Datenmodelle";
            OrdnerEreignismodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Ereignismodelle";
            OrdnerFunktionsmodelle = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Funktionsmodelle";
            OrdnerSchnittstellen = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Schnittstellen";
            OrdnerBeschreibungen = Environment.GetEnvironmentVariable("AUTOMATISIERUNG_ANWENDUNGEN") + "/" + Anwendung + "/Beschreibungen";

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