using System;
using System.Linq;
using System.Collections.Generic;
using isci.Anwendungen;
using isci.Allgemein;
using isci.Daten;

namespace isci.Beschreibung
{
    public class Schnittstelle : Header
    {
        public string Automatisierungsressource;

        public Schnittstelle() : base() { }

        public Schnittstelle(string Identifikation, string Ressource, string Name = "", string Beschreibung = "", string Typ = "") : base(Identifikation, Name, Beschreibung, Typ)
        {
            Automatisierungsressource = Ressource;
        }
    }

    public class SchnittstelleUdp : Schnittstelle
    {
        public string adresse;
        public int sendPort;
        public int receivePort;

        public SchnittstelleUdp() : base() { }

        public SchnittstelleUdp(string Identifikation, string adresse, int sendPort, int receivePort, string Ressource, string Name = "", string Beschreibung = "", string Typ = "") : base(Identifikation, Ressource, Name, Beschreibung, Typ)
        {
            this.adresse = adresse;
            this.sendPort = sendPort;
            this.receivePort = receivePort;
        }
    }
}