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
        public Dictionary<string, List<Entdeckung>> Anwendungen_Entdeckungen = new Dictionary<string, List<Entdeckung>>();

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

        Dictionary<string, DomainName> targets = new Dictionary<string, DomainName>();

        public void Bewerben(Allgemein.Parameter parameter, string Modultyp, int Port)
        {
            Bewerben(parameter.Anwendung, parameter.Identifikation, parameter.Ressource, Modultyp, Port);
        }

        public void Bewerben(string Anwendung, string Identifikation, string Ressource, string Modultyp, int Port)
        {
            var service = new ServiceProfile(Anwendung, Identifikation + ".isci", (ushort)Port);
            service.AddProperty("Ressource", Ressource);
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

                // Is this an answer to host addresses?
                var addresses = e.Message.Answers.OfType<AddressRecord>();
                foreach (var address in addresses)
                {
                    if (!address.CanonicalName.Contains(".isci.")) continue;
                    if (!Entdeckungen.ContainsKey(address.CanonicalName))
                    {
                        Entdeckungen.Add(address.CanonicalName, new Entdeckung(){CanonicalName = address.CanonicalName});
                    }
                    var geteilt = address.CanonicalName.Split('.');
                    
                    Entdeckungen[address.CanonicalName].Ipv4 = address.Address;
                    Entdeckungen[address.CanonicalName].Anwendung = geteilt[0];
                    Entdeckungen[address.CanonicalName].Identifikation = geteilt[1];

                    if (!Anwendungen_Entdeckungen.ContainsKey(geteilt[0])) Anwendungen_Entdeckungen.Add(geteilt[0], new List<Entdeckung>() {});
                    if (!Anwendungen_Entdeckungen[geteilt[0]].Contains(Entdeckungen[address.CanonicalName])) Anwendungen_Entdeckungen[geteilt[0]].Add(Entdeckungen[address.CanonicalName]);
                    //if (address.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) //wir fragen eh bloß nach ipv4

                    mdns.SendQuery(targets[address.CanonicalName], type: DnsType.TXT);
                }

                var props = e.Message.Answers.OfType<TXTRecord>();
                foreach (var prop in props)
                {
                    if (!prop.CanonicalName.Contains(".isci.")) continue;
                    if (!Entdeckungen.ContainsKey(prop.CanonicalName))
                    {
                        Entdeckungen.Add(prop.CanonicalName, new Entdeckung(){CanonicalName = prop.CanonicalName});
                    }
                    var geteilt = prop.CanonicalName.Split('.');
                    Entdeckungen[prop.CanonicalName].Anwendung = geteilt[0];
                    Entdeckungen[prop.CanonicalName].Identifikation = geteilt[1];

                    if (!Anwendungen_Entdeckungen.ContainsKey(geteilt[0])) Anwendungen_Entdeckungen.Add(geteilt[0], new List<Entdeckung>() {});
                    if (!Anwendungen_Entdeckungen[geteilt[0]].Contains(Entdeckungen[prop.CanonicalName])) Anwendungen_Entdeckungen[geteilt[0]].Add(Entdeckungen[prop.CanonicalName]);

                    foreach (var p_ in prop.Strings)
                    {
                        geteilt = p_.Split('=');
                        switch (geteilt[0])
                        {
                            case "Modul": Entdeckungen[prop.CanonicalName].Modul = geteilt[1]; break;
                            case "Port": Entdeckungen[prop.CanonicalName].Port = geteilt[1];break;
                            case "Ressource": Entdeckungen[prop.CanonicalName].Ressource = geteilt[1];break;
                            default: break;
                        }
                    }
                }
            };
        }

        public mdnsDiscovery()
        {
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