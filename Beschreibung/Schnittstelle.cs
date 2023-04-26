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

        public SchnittstelleUdp() : base() { }

        public SchnittstelleUdp(string Identifikation, string adresse, string Ressource, string Name = "", string Beschreibung = "", string Typ = "")  : base(Identifikation, Ressource, Name, Beschreibung, Typ)
        {
            this.adresse = adresse;
        }
    }
}