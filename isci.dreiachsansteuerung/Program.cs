using System;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;
using isci.Daten;
using isci.Beschreibung;

namespace isci.dreiachsansteuerung
{
    class Program
    {
        public class Konfiguration : Parameter
        {
            public uint AchseXSteigung, AchseYSteigung, AchseZSteigung;
            public uint AchseXSchritte, AchseYSchritte, AchseZSchritte;
            public bool AchseXDrehrichtung, AchseYDrehrichtung, AchseZDrehrichtung;
            public string Port;
            public uint Baudrate;

            public Konfiguration(string datei) : base(datei) { }
        }

        static void Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");

            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "/bin/stty";
                startInfo.Arguments = "-F " + konfiguration.Port + " " + konfiguration.Baudrate + " cs8 -cstopb -parenb";
                process.StartInfo = startInfo;
                process.Start();
            }

            System.Threading.Thread.Sleep(100);
            
            var structure = new Datenstruktur(konfiguration.OrdnerDatenstruktur);

            var dm = new Datenmodell(konfiguration.Identifikation);
            var delta_s_x = new dtFloat(0F, "delta_s_x");
            var v_x = new dtFloat(0F, "v_x");
            var a_x = new dtFloat(0F, "a_x");
            dm.Dateneinträge.Add(delta_s_x);
            dm.Dateneinträge.Add(v_x);
            dm.Dateneinträge.Add(a_x);

            var delta_s_y = new dtFloat(0F, "delta_s_y");
            var v_y = new dtFloat(0F, "v_y");
            var a_y = new dtFloat(0F, "a_y");
            dm.Dateneinträge.Add(delta_s_y);
            dm.Dateneinträge.Add(v_y);
            dm.Dateneinträge.Add(a_y);

            var delta_s_z = new dtFloat(0F, "delta_s_z");
            var v_z = new dtFloat(0F, "v_z");
            var a_z = new dtFloat(0F, "a_z");
            dm.Dateneinträge.Add(delta_s_z);
            dm.Dateneinträge.Add(v_z);
            dm.Dateneinträge.Add(a_z);

            var beschreibung = new Modul(konfiguration.Identifikation, "isci.dreiachsansteuerung", dm.Dateneinträge);
            beschreibung.Name = "Dreiachsansteuerung " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Modul zur dreiachsansteuerung über Arduino USB";
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");

            dm.Speichern(konfiguration.OrdnerDatenmodelle + "/" + konfiguration.Identifikation + ".json");

            structure.DatenmodellEinhängen(dm);
            structure.Start();

            var Zustand = new dtInt32(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            structure.Lesen();

            while(true)
            {
                System.Threading.Thread.Sleep(10);
                Zustand.Lesen();

                var erfüllteTransitionen = konfiguration.Ausführungstransitionen.Where(a => a.Eingangszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<Ausführungstransition>() > 0)
                {
                    var r = structure.Lesen();

                    //if (structure.AenderungVorhanden())
                    if (r.Count > 0)
                    {
                        structure.AenderungenZuruecksetzen(r);

                        var schritteX_ = ((float)delta_s_x.value / konfiguration.AchseXSteigung * konfiguration.AchseXSchritte);
                        var schritteX = (konfiguration.AchseXDrehrichtung ? 1 : -1) * (int)Math.Round(schritteX_, 0, MidpointRounding.AwayFromZero);
                        var geschwindigkeitX = (int)Math.Round( (float)v_x.value / (float)konfiguration.AchseXSteigung * (float)konfiguration.AchseXSchritte, 0, MidpointRounding.AwayFromZero);
                        var beschleunigungX = (int)Math.Round( (float)a_x.value / (float)konfiguration.AchseXSteigung * (float)konfiguration.AchseXSchritte, 0, MidpointRounding.AwayFromZero);

                        var schritteY_ = ((float)delta_s_y.value / konfiguration.AchseYSteigung * konfiguration.AchseYSchritte);
                        var schritteY = (konfiguration.AchseYDrehrichtung ? 1 : -1) * (int)Math.Round(schritteY_, 0, MidpointRounding.AwayFromZero);
                        var geschwindigkeitY = (float)v_y.value / (float)konfiguration.AchseYSteigung * (float)konfiguration.AchseYSchritte;
                        var beschleunigungY = (float)a_y.value / (float)konfiguration.AchseYSteigung * (float)konfiguration.AchseYSchritte;

                        var schritteZ_ = ((float)delta_s_z.value / konfiguration.AchseZSteigung * konfiguration.AchseZSchritte);
                        var schritteZ = (konfiguration.AchseZDrehrichtung ? 1 : -1) * (int)Math.Round(schritteZ_, 0, MidpointRounding.AwayFromZero);
                        var geschwindigkeitZ = (float)v_z.value / (float)konfiguration.AchseZSteigung * (float)konfiguration.AchseZSchritte;
                        var beschleunigungZ = (float)a_z.value / (float)konfiguration.AchseZSteigung * (float)konfiguration.AchseZSchritte;

                        var nachricht = "{";
                        nachricht += "\"c\":3,";
                        nachricht += "\"1\":[" + schritteX.ToString().Replace(',', '.') + "," + geschwindigkeitX.ToString().Replace(',', '.') + "," + beschleunigungX.ToString().Replace(',', '.') + "],";
                        nachricht += "\"2\":[" + schritteY.ToString().Replace(',', '.') + "," + geschwindigkeitY.ToString().Replace(',', '.') + "," + beschleunigungY.ToString().Replace(',', '.') + "],";
                        nachricht += "\"3\":[" + schritteZ.ToString().Replace(',', '.') + "," + geschwindigkeitZ.ToString().Replace(',', '.') + "," + beschleunigungZ.ToString().Replace(',', '.') + "]";
                        nachricht += "}\n";

                        System.IO.File.WriteAllText(konfiguration.Port, nachricht);

                        delta_s_x.value = 0F;
                        delta_s_y.value = 0F;
                        delta_s_z.value = 0F;
                        structure.Schreiben();
                    }

                    Zustand.value = erfüllteTransitionen.First<Ausführungstransition>().Ausgangszustand;
                    Zustand.Schreiben();
                }
            }   
        }
    }
}
