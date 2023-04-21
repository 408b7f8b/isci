﻿﻿using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.IO.Ports;
using isci.Allgemein;
using isci.Daten;
using isci.Beschreibung;

namespace isci.modbusspindel
{
    class Program
    {
        static System.Byte genLRC(List<System.Byte> content)
        {
            System.Byte tmp = 0;
            for (int i = 0; i < content.Count; ++i)
                tmp += content[i];

            System.Byte ret = (System.Byte)(0xFF - tmp + 1);
            return ret;
        }

        static void uint16tNachUint8ts(System.UInt16 uint16, ref System.Byte most8bit, ref System.Byte least8bit)
        {
            most8bit = (System.Byte)(uint16 >> 8);
            least8bit = (System.Byte)(uint16 & 0x00FF);
        }

        static List<System.Byte> bauMirDochBitteEineASCIIKette(List<System.Byte> bytekette)
        {
            var ziel = new List<System.Byte>();
            for (int pos = 0; pos < bytekette.Count; ++pos)
            {
                var tmp = bytekette[pos] >> 4;
                ziel.Add((System.Byte)(tmp > 9 ? tmp + 0x37 : tmp + 0x30));

                tmp = bytekette[pos] & 0x0F;
                ziel.Add((System.Byte)(tmp > 9 ? tmp + 0x37 : tmp + 0x30));
            }

            return ziel;
        }

        public class Nachricht {
            System.Byte Adresse_Zielsystem;
            System.Byte Befehl;
            public System.UInt16 Adresse_Speicher_Zielsystem;
            public List<System.UInt16> Woerter;

            public Nachricht(System.Byte Adresse_Zielsystem, System.Byte Befehl, System.UInt16 Adresse_Speicher_Zielsystem, List<System.UInt16> Woerter)
            {
                this.Adresse_Zielsystem = Adresse_Zielsystem;
                this.Befehl = Befehl;
                this.Adresse_Speicher_Zielsystem = Adresse_Speicher_Zielsystem;
                this.Woerter = Woerter;
            }

            public List<System.Byte> KetteBauen()
            {
                List<System.Byte> Kette = new List<byte>();

                Kette.Add(Adresse_Zielsystem);
                Kette.Add(Befehl);
                var b1 = new System.Byte();
                var b2 = new System.Byte();
                uint16tNachUint8ts(Adresse_Speicher_Zielsystem, ref b1, ref b2);
                Kette.Add(b1);
                Kette.Add(b2);

                if (Woerter.Count > 1)
                {
                    Kette.Add(0);
                    Kette.Add((System.Byte)Woerter.Count);
                    Kette.Add((System.Byte)Woerter.Count);
                    for (int wort = 0; wort < Woerter.Count; ++wort)
                    {
                        var b_1 = new System.Byte();
                        var b_2 = new System.Byte();
                        uint16tNachUint8ts(Woerter[wort], ref b_1, ref b_2);
                        Kette.Add(b_1);
                        Kette.Add(b_2);
                    }
                } else {
                    var b_1 = new System.Byte();
                    var b_2 = new System.Byte();
                    uint16tNachUint8ts(Woerter[0], ref b_1, ref b_2);
                    Kette.Add(b_1);
                    Kette.Add(b_2);
                }
                
                Kette.Add(genLRC(Kette));
                
                var ascii = bauMirDochBitteEineASCIIKette(Kette);
                ascii.Insert(0, 0x3A);

                ascii.Add(0x0D);
                ascii.Add(0x0A);
                
                return ascii;
            }
        };

        public class Konfiguration : Parameter
        {
            public string Port;
            public uint Baudrate;

            public Konfiguration(string datei) : base(datei) { }
        }

        static void Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");
            
            var structure = new Datenstruktur(konfiguration.OrdnerDatenstruktur);

            //port /dev/ttyRS485

            using (System.Diagnostics.Process process = new System.Diagnostics.Process())
            {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "/usr/bin/stty";
                startInfo.Arguments = "-F " + konfiguration.Port + " " + konfiguration.Baudrate + " cs8 -cstopb -parodd";
                process.StartInfo = startInfo;
                process.Start();
            }

            System.Threading.Thread.Sleep(100);

            var n = new Nachricht(1, 6, 0, new List<ushort>(){0x0030});

            using (null)
            {
                var arr = n.KetteBauen().ToArray();
                System.IO.File.WriteAllBytes(konfiguration.Port, arr);
                System.Threading.Thread.Sleep(100);
            }

            var dm = new Datenmodell(konfiguration.Identifikation);
            var beschleunigungszeit = new dtUInt16(1000, "zeitInMsFuer1000UpmA");
            var drehzahl = new dtInt16(0, "Drehzahl");
            dm.Dateneinträge.Add(beschleunigungszeit);
            dm.Dateneinträge.Add(drehzahl);

            var beschreibung = new Modul(konfiguration.Identifikation, "isci.modbusspindel", dm.Dateneinträge);
            beschreibung.Name = "Spindel via Modbus ASCII " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Modul zur Spindelansteuerung über Modbus ASCII";
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");

            dm.Speichern(konfiguration.OrdnerDatenmodelle + "/" + konfiguration.Identifikation + ".json");

            structure.DatenmodellEinhängen(dm);
            structure.Start();

            var Zustand = new dtInt32(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            while(true)
            {
                Zustand.Lesen();

                var erfüllteTransitionen = konfiguration.Ausführungstransitionen.Where(a => a.Eingangszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<Ausführungstransition>() > 0)
                {
                    structure.Lesen();
                    if (beschleunigungszeit.aenderung)
                    {
                        n.Adresse_Speicher_Zielsystem = 303;
                        n.Woerter[0] = (System.UInt16)beschleunigungszeit.value;

                        using (null)
                        {
                            var arr = n.KetteBauen().ToArray();
                            System.IO.File.WriteAllBytes(konfiguration.Port, arr);
                            n.Adresse_Speicher_Zielsystem = 304;
                            arr = n.KetteBauen().ToArray();
                            System.Threading.Thread.Sleep(30);
                            System.IO.File.WriteAllBytes(konfiguration.Port, arr);
                        }

                        beschleunigungszeit.aenderung = false;
                    }
                    if (drehzahl.aenderung)
                    {
                        n.Adresse_Speicher_Zielsystem = 307;

                        System.UInt16 drehzahl_tmp;
                        if ((System.Int16)drehzahl.value >= 0)
                        {
                            drehzahl_tmp = (System.UInt16)drehzahl.value;
                        } else {
                            drehzahl_tmp = ((System.UInt16)(-1*(System.Int16)drehzahl.value));
                            drehzahl_tmp = (System.UInt16)(0xFFFF - drehzahl_tmp + 1);
                        }
                        n.Woerter[0] = drehzahl_tmp;

                        using (null)
                        {
                            var arr = n.KetteBauen().ToArray();
                            System.IO.File.WriteAllBytes(konfiguration.Port, arr);
                        }

                        drehzahl.aenderung = false;
                    }

                    Zustand.value = erfüllteTransitionen.First<Ausführungstransition>().Ausgangszustand;
                    Zustand.Schreiben();
                }
            }           

            /*System.Threading.Thread.Sleep(100);

            var n_einrichten = new Nachricht(1, 6, 0, new List<ushort>(){0x0030}).KetteBauen().ToArray();
            var n_beschl1 = new Nachricht(1, 6, 303, new List<ushort>(){150}).KetteBauen().ToArray();
            var n_beschl2 = new Nachricht(1, 6, 304, new List<ushort>(){150}).KetteBauen().ToArray();
            var n_drehzahl1 = new Nachricht(1, 6, 307, new List<ushort>(){1500}).KetteBauen().ToArray();
            var n_drehzahl2 = new Nachricht(1, 6, 307, new List<ushort>(){5}).KetteBauen().ToArray();
            var n_drehzahl3 = new Nachricht(1, 6, 307, new List<ushort>(){0}).KetteBauen().ToArray();

            System.IO.File.WriteAllBytes("/dev/ttyRS485", n_einrichten);System.Threading.Thread.Sleep(100);
            System.IO.File.WriteAllBytes("/dev/ttyRS485", n_beschl1);System.Threading.Thread.Sleep(100);
            System.IO.File.WriteAllBytes("/dev/ttyRS485", n_beschl2);System.Threading.Thread.Sleep(100);

            for (int i = 0; i < 5; i++)
            {
                System.IO.File.WriteAllBytes("/dev/ttyRS485", n_drehzahl1);System.Threading.Thread.Sleep(3000);
                System.IO.File.WriteAllBytes("/dev/ttyRS485", n_drehzahl2);System.Threading.Thread.Sleep(3000);
            }

            System.IO.File.WriteAllBytes("/dev/ttyRS485", n_drehzahl3);*/            

            /*System.Threading.Thread.Sleep(100);
            new Nachricht(1, 6, 303, new List<ushort>(){1000}).Senden();System.Threading.Thread.Sleep(100);
            new Nachricht(1, 6, 304, new List<ushort>(){1000}).Senden();System.Threading.Thread.Sleep(100);
            new Nachricht(1, 6, 307, new List<ushort>(){1000}).Senden();System.Threading.Thread.Sleep(2000);
            new Nachricht(1, 6, 307, new List<ushort>(){0}).Senden();*/
        }
    }
}