using System;
using System.Net.Sockets;
using System.Net;
using System.Linq;

namespace integration
{
    public class Konfiguration : library.Konfiguration
    {
        public string Target;
        public int Port;

        public Konfiguration(string datei) : base(datei)
        {

        }
    }

    class Program
    {
        static System.Net.Sockets.Socket udpSock;
        static byte[] buffer = new byte[1024];
        static EndPoint endpoint = new IPEndPoint(IPAddress.Any, 0);
        static void udpStart(){
            udpSock = new System.Net.Sockets.Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSock.Bind(new System.Net.IPEndPoint(IPAddress.Any, 1337));
            udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint, udpCallback, udpSock);
        }

        static System.Collections.Generic.Dictionary<string, string> aenderungen = new System.Collections.Generic.Dictionary<string, string>();

        static void udpCallback(IAsyncResult asyncResult){
            try
            {
                Socket recvSock = (Socket)asyncResult.AsyncState;
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);
                int length = recvSock.EndReceiveFrom(asyncResult, ref client);
                var conv_string = System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, length);
                conv_string = conv_string.TrimEnd();

                var string_parts = conv_string.Split('#');
                while(change_lock) {}
                try
                {
                    if (aenderungen.ContainsKey(string_parts[0])) aenderungen[string_parts[0]] = string_parts[1];
                    else aenderungen.Add(string_parts[0], string_parts[1]);
                } catch {

                }
        
                EndPoint endpoint = target;//new IPEndPoint(target, 0);
                udpSock.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref endpoint, udpCallback, udpSock);
            } catch {
                
            }
        }

        static System.Net.IPEndPoint target;
        static bool change_lock;
        static library.Datastructure structure;

        static void Main(string[] args)
        {
            var konfiguration = new Konfiguration("konfiguration.json");
            
            structure = new library.Datastructure(konfiguration.OrdnerDatenstruktur);

            var dm = new library.Datamodel(konfiguration.Identifikation);
            var cycle = new library.var_int(0, "cycle");
            dm.Datafields.Add(cycle);

            var beschreibung = new Beschreibung.Modul();
            beschreibung.Identifikation = konfiguration.Identifikation;
            beschreibung.Name = "Integration Ressource " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Modul zur Integration";
            beschreibung.Typidentifikation = "isci.integration";
            beschreibung.Datenfelder = new library.FieldList(){cycle};
            beschreibung.Ereignisse = new System.Collections.Generic.List<library.Ereignis>();
            beschreibung.Funktionen = new System.Collections.Generic.List<library.Funktion>();
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");

            System.IO.File.WriteAllText(konfiguration.OrdnerDatenmodelle + "/" + konfiguration.Identifikation + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(dm));

            structure.AddDatamodel(dm);
            structure.AddDataModelsFromDirectory(konfiguration.OrdnerDatenmodelle);
            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            target = System.Net.IPEndPoint.Parse(konfiguration.Target);
            target.Port = konfiguration.Port;

            udpStart();
            long curr_ticks = 0;
            
            while(true)
            {
                Zustand.WertLesen();

                var erfüllteTransitionen = konfiguration.Zustandsbereiche.Where(a => a.Arbeitszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<library.Zustandsbereich>() > 0)
                {
                    if ((System.Int32)Zustand.value == 0)
                    {
                        var curr_ticks_new = System.DateTime.Now.Ticks;
                        var ticks_span = curr_ticks_new - curr_ticks;
                        curr_ticks = curr_ticks_new;
                        cycle.value = (System.Int32)(ticks_span / System.TimeSpan.TicksPerMillisecond);
                        cycle.WertSchreiben();

                        change_lock = true;
                        foreach (var aenderung in aenderungen)
                        {
                            try {
                                structure.Datafields[aenderung.Key].WertAusString(aenderung.Value);
                                structure.Datafields[aenderung.Key].WertSchreiben();
                            } catch {

                            }                            
                        }
                        aenderungen.Clear();
                        change_lock = false;
                    } else {
                        var updated = structure.UpdateImage();

                        foreach (var entry in updated)
                        {
                            if (structure.Datafields[entry].Identifikation == "ns=integration;cycle") continue;
                            var snd = System.Text.UTF8Encoding.UTF8.GetBytes(entry + "#" + structure.Datafields[entry].WertSerialisieren());
                            udpSock.BeginSendTo(snd, 0, snd.Length, 0, target, null, null);//   (snd, new System.Net.IPEndPoint(IPAddress.Any, 1337));
                            structure.Datafields[entry].aenderung = false;
                        }
                    }

                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}
