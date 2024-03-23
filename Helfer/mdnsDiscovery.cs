using Makaretu.Dns;
using System;
using System.Collections.Generic;
using System.Linq;

namespace isci
{
    public class mdnsDiscovery
    {
        public MulticastService mdns;
        public ServiceDiscovery sd;

        public Dictionary<string, Entdeckung> Entdeckungen = new Dictionary<string, Entdeckung>();
        public Dictionary<string, List<Entdeckung>> Anwendungen_ZugehoerigeEntdeckungen = new Dictionary<string, List<Entdeckung>>();

        System.Threading.Thread thread;
        bool thread_beenden;

        public class Entdeckung
        {
            public string CanonicalName;
            public System.Net.IPAddress Ipv4;
            public string Port;
            public string Identifikation;
            public string Ressource;
            public string Anwendung;
            public string Modul;
        }

        Allgemein.Parameter parameter;
        string Modultyp;
        int Port;
        Dictionary<string, DomainName> targets = new Dictionary<string, DomainName>();

        public void Bewerben(Allgemein.Parameter parameter, string Modultyp, int Port)
        {
            this.parameter = parameter;
            this.Modultyp = Modultyp;
            this.Port = Port;
            Bewerben();
        }

        /* public void Bewerben(string Anwendung, string Identifikation, string Ressource, string Modultyp, int Port)
        {
            var service = new ServiceProfile(Anwendung, Identifikation + ".isci", (ushort)Port);
            service.AddProperty("Ressource", Ressource);
            service.AddProperty("Modul", "isci." + Modultyp);
            service.AddProperty("Port", Port.ToString());
            var sd = new ServiceDiscovery();
            sd.Advertise(service);
        } */

        public void Bewerben()
        {
            var service = new ServiceProfile(parameter.Anwendung, parameter.Identifikation + ".isci", (ushort)Port);
            service.AddProperty("Ressource", parameter.Ressource);
            service.AddProperty("Modul", "isci." + Modultyp);
            service.AddProperty("Port", Port.ToString());
            var sd = new ServiceDiscovery();
            sd.Advertise(service);
        }

        public void Entdecken()
        {
            mdns.NetworkInterfaceDiscovered += (s, e) =>
            {
                sd.QueryAllServices();
            };

            sd.ServiceDiscovered += (s, serviceName) =>
            {
                var sName = $"{serviceName}";

                if (sName.Contains(".isci."))
                {
                    mdns.SendQuery(serviceName, type: DnsType.PTR);
                }
            };

            sd.ServiceInstanceDiscovered += (s, e) =>
            {
                var sIName = $"{e.ServiceInstanceName}";

                if (sIName.Contains(".isci."))
                    mdns.SendQuery(e.ServiceInstanceName, type: DnsType.SRV);
            };

            mdns.AnswerReceived += (s, e) =>
            {
                if (e.Message.Answers.Count == 2)
                {
                    if (e.Message.Answers[0].CanonicalName.Contains(".isci."))
                    {
                        var address = e.Message.Answers.OfType<AddressRecord>().First();
                        var textRecord = e.Message.Answers.OfType<TXTRecord>().First();
                        
                        var geteilt = address.CanonicalName.Split('.');
                        Entdeckung neueEntdeckung = new Entdeckung
                        {
                            Ipv4 = address.Address,
                            Anwendung = geteilt[0],
                            Identifikation = geteilt[1],
                            CanonicalName = address.CanonicalName
                        };
                        foreach (var Eintrag in textRecord.Strings)
                        {
                            geteilt = Eintrag.Split('=');
                            switch (geteilt[0])
                            {
                                case "Modul": neueEntdeckung.Modul = geteilt[1]; break;
                                case "Port": neueEntdeckung.Port = geteilt[1]; break;
                                case "Ressource": neueEntdeckung.Ressource = geteilt[1]; break;
                                default: break;
                            }
                        }
                        // Modellaustausch bei gleicher IP verhindern
                        var eigeneIP = MulticastService.GetIPAddresses().Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First();
                        if (neueEntdeckung.Ipv4.Equals(eigeneIP)) return;
                        if(!Anwendungen_ZugehoerigeEntdeckungen.ContainsKey(neueEntdeckung.Anwendung)){
                            // Wenn Anwendung des gefundenen Modulls noch nie gefunden wurde, dann erstelle sie
                            Anwendungen_ZugehoerigeEntdeckungen.Add(neueEntdeckung.Anwendung, new List<Entdeckung>());
                        }
                        if(!Anwendungen_ZugehoerigeEntdeckungen[neueEntdeckung.Anwendung].Contains(neueEntdeckung)){
                            // Wenn das gefundene Modull neu ist, füge es der Liste hinzu
                            Anwendungen_ZugehoerigeEntdeckungen[neueEntdeckung.Anwendung].Add(neueEntdeckung);
                        }
                    }
                }
                // Is this an answer to a service instance details?
                var servers = e.Message.Answers.OfType<SRVRecord>();
                foreach (var server in servers)
                {
                    if (!server.CanonicalName.Contains(".isci.")) continue;
                    if (targets.ContainsKey(server.CanonicalName)) continue;
                    // Ask for the host IP addresses.
                    mdns.SendQuery(server.Target, type: DnsType.A); //frage nach ipv4
                    targets.Add(server.CanonicalName, server.Target);
                    //mdns.SendQuery(server.Target, type: DnsType.AAAA); //frage nach ipv6
                }
            };

            mdns.QueryReceived += (s, e) =>
            {
                var msg = e.Message;
                foreach (var Question in msg.Questions)
                {
                    if (Question.Name.Labels.Contains("isci"))
                    {
                        Message antwort = msg.CreateResponse();
                        antwort.Answers.Add(new TXTRecord
                        {
                            Name = parameter.Anwendung + "." + parameter.Identifikation + ".isci." + Modultyp,
                            Strings = new List<string>{
                                "Modul=" + Modultyp,
                                "Port=" + Port.ToString(),
                                "Ressource="+parameter.Ressource }
                        });
                        antwort.Answers.Add(new ARecord
                        {
                            Name = parameter.Anwendung + "." + parameter.Identifikation + ".isci." + Modultyp,
                            Address = MulticastService.GetIPAddresses().Where(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).First()
                        });
                        mdns.SendAnswer(antwort);
                    }
                }
            };
        }

        public mdnsDiscovery(Allgemein.Parameter InputParameter, string InputModultyp, int InputPort)
        {
            parameter = InputParameter;
            Modultyp = InputModultyp;
            Port = InputPort;

            mdns = new MulticastService();
            sd = new ServiceDiscovery(mdns);
        }

        public void starteThread()
        {
            thread_beenden = false;
            thread = new System.Threading.Thread(this.work);
            thread.Start();
        }

        public void beendeThread()
        {
            thread_beenden = true;
            thread.Join();
        }

        public void work()
        {
            mdns.Start();
            while(!thread_beenden)
            {
                System.Threading.Thread.Sleep(10000);
            }
            mdns.Stop();
        }
    }
}