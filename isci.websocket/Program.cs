using System;
using System.Threading;
using System.Linq;
using System.Collections.Generic;
using isci.Allgemein;
using isci.Daten;
using isci.Beschreibung;

namespace isci.websocket
{
    public class Konfiguration : Parameter
    {
        public int Port;
        public string[] Ausschliessen;

        public Konfiguration(string datei) : base(datei) {

        }
    }

    class Program
    {
        static Datenstruktur structure;
        static bool msg_block = false;

        static void Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");
            if (konfiguration.Ausführungstransitionen.Count() != 2) return;

            var beschreibung = new Modul(konfiguration.Identifikation, "isci.websocket", new ListeDateneintraege(){ });
            beschreibung.Name = "Websocket Ressource " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Websocket-Servermodul";
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");

            structure = new Datenstruktur(konfiguration.OrdnerDatenstruktur);
            structure.DatenmodelleEinhängenAusOrdner(konfiguration.OrdnerDatenmodelle);

            foreach (var aus in konfiguration.Ausschliessen) if (structure.dateneinträge.ContainsKey(aus)) structure.dateneinträge.Remove(aus);

            structure.Start();

            var Zustand = new dtInt32(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            var schreiben = new List<string>();
            
            var server = new WebSocketServer.Server(new System.Net.IPEndPoint(System.Net.IPAddress.Any, konfiguration.Port));

            // Bind the event for when a client connected
            server.OnClientConnected += (object sender, WebSocketServer.OnClientConnectedHandler e) =>
            {
                string clientGuid = e.GetClient().GetGuid();
                Console.WriteLine("Client with guid {0} connected!", clientGuid);
            };

            server.OnClientDisconnected += (object sender, WebSocketServer.OnClientDisconnectedHandler e) => 
            {
                string clientGuid = e.GetClient().GetGuid();
                Console.WriteLine("Client with guid {0} disconnected!", clientGuid);
            };

            // Bind the event for when a message is received
            server.OnMessageReceived += (object sender, WebSocketServer.OnMessageReceivedHandler e) =>
            {
                msg_block = true;

                try {
                    var arr = Newtonsoft.Json.Linq.JObject.Parse(e.GetMessage());

                    foreach (var eintrag in arr)
                    {
                        if (structure.dateneinträge.ContainsKey(eintrag.Key))
                        {
                            structure.dateneinträge[eintrag.Key].AusJToken(eintrag.Value);
                            if (!schreiben.Contains(eintrag.Key)) schreiben.Add(eintrag.Key);
                        }
                    }
                } catch {

                }

                msg_block = false;
                
                Console.WriteLine("Message received: {0}", e.GetMessage());
            };
            
            while(true)
            {
                System.Threading.Thread.Sleep(10);
                Zustand.Lesen();

                var erfüllteTransitionen = konfiguration.Ausführungstransitionen.Where(a => a.Eingangszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<Ausführungstransition>() > 0)
                {
                    if (erfüllteTransitionen.First() == konfiguration.Ausführungstransitionen.First())
                    {
                        structure.Lesen();

                        var obj = new Newtonsoft.Json.Linq.JObject();

                        foreach (var eintrag in structure.dateneinträge)
                        {
                            if (eintrag.Value.aenderung)
                            {
                                obj.Add(eintrag.Key, eintrag.Value.NachJToken());
                            }
                        }

                        /*if (obj.Properties().Count() > 0) 
                        {
                            var nachricht = obj.ToString();
                            server.Broadcast(nachricht);
                        }*/
                    } else if (erfüllteTransitionen.First() == konfiguration.Ausführungstransitionen.Last()) {
                        while (msg_block) { }
                        
                        foreach (var eintrag in schreiben)
                        {
                            structure.dateneinträge[eintrag].Schreiben();
                        }
                        schreiben.Clear();
                    }

                    Zustand.value = erfüllteTransitionen.First<Ausführungstransition>().Ausgangszustand;
                    Zustand.Schreiben();
                }
            }
        }
    }
}