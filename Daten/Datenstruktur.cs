using System;
using System.Linq;

namespace isci.Daten
{
    public class Datenstruktur
    {
        public System.Collections.Generic.List<string> datenmodelle;
        public System.Collections.Generic.Dictionary<string, Dateneintrag> dateneinträge;
        private VerweislisteDateneintraege verweise;
        public VerweislisteDateneintraegeAktiv verweiseAktiv;
        public string pfad;

        public Datenstruktur(string pfad)
        {
            datenmodelle = new System.Collections.Generic.List<string>();
            dateneinträge = new System.Collections.Generic.Dictionary<string, Dateneintrag>();
            verweise = new VerweislisteDateneintraege();
            verweiseAktiv = new VerweislisteDateneintraegeAktiv();
            this.pfad = pfad;
        }

        /*public void DatenmodellEinhängen(Datenmodell datenmodell)
        {
            try
            {
                foreach (var eintrag in datenmodell.Dateneinträge)
                {
                    string ident = "ns=" + datenmodell.Identifikation + ";s=" + eintrag.Identifikation;
                    eintrag.Identifikation = ident;
                    eintrag.path = pfad + "/" + eintrag.Identifikation;
                    dateneinträge.Add(ident, eintrag);
                }
                foreach (var eintrag in datenmodell.Links)
                {
                    var Links_ = new ListeDateneintraege();
                    var key = eintrag.Key;
                    var value = new System.Collections.Generic.List<string>();
                    if (!key.StartsWith("ns="))
                    {
                        key = "ns=" + datenmodell.Identifikation + ";s=" + key;
                    }
                    foreach (var untereintrag in eintrag.Value)
                    {
                        if (untereintrag.StartsWith("ns=")) value.Add(untereintrag);
                        else value.Add("ns=" + datenmodell.Identifikation + ";s=" + untereintrag);
                    }
                    verweise.Add(key, value);
                }
                datenmodelle.Add(datenmodell.Identifikation);
            } catch (Exception)
            {
                
            }            
        }*/

        public void DatenmodellEinhängen(Datenmodell datenmodell)
        {
            try
            {
                foreach (var eintrag in datenmodell.Dateneinträge)
                {
                    string ident = "ns=2;s=" + datenmodell.Identifikation + "." + eintrag.Identifikation;
                    eintrag.Identifikation = ident;
                    eintrag.path = pfad + "/" + eintrag.Identifikation;
                    dateneinträge.Add(ident, eintrag);
                }
                foreach (var eintrag in datenmodell.Links)
                {
                    var Links_ = new ListeDateneintraege();
                    var key = eintrag.Key;
                    var value = new System.Collections.Generic.List<string>();
                    if (!key.StartsWith("ns="))
                    {
                        key = "ns=2;s=" + datenmodell.Identifikation + "." + key;
                    }
                    foreach (var untereintrag in eintrag.Value)
                    {
                        if (untereintrag.StartsWith("ns=")) value.Add(untereintrag);
                        else value.Add("ns=2;s=" + datenmodell.Identifikation + "." + untereintrag);
                    }
                    verweise.Add(key, value);
                }
                datenmodelle.Add(datenmodell.Identifikation);
            } catch (Exception)
            {
                
            }            
        }

        public void DatenmodellEinhängenAusDatei(string pfad)
        {
            var dm_ = Datenmodell.AusDatei(pfad);
            DatenmodellEinhängen(dm_);
        }

        public void DatenmodelleEinhängenAusOrdner(string pfade, string excludeown = "")
        {
            if (pfade == null)
            {
                Console.WriteLine("DatenmodelleEinhängenAusOrdner: pfade ist NULL");
                return;
            }
            
            var dms_ = System.IO.Directory.GetFiles(pfade);
            foreach (var dm_ in dms_)
            {
                if (excludeown != "")
                {
                    if (dm_.Contains(excludeown)) continue;
                }                
                DatenmodellEinhängenAusDatei(dm_);
            }
        }

        public void DatenmodelleEinhängenAusOrdnern(string[] pfade, string excludeown = "")
        {
            foreach (var pfad in pfade)
            {
                DatenmodelleEinhängenAusOrdner(pfad, excludeown);
            }
        }

        public void VerweiseErzeugen()
        {
            foreach (var eintrag in verweise)
            {
                foreach (var subentry in eintrag.Value)
                {
                    try {
                        verweiseAktiv.Add(dateneinträge[eintrag.Key], dateneinträge[subentry]);
                    } catch {

                    }
                }
            }
        }

        public void Start()
        {
            foreach (var eintrag in dateneinträge)
            {
                eintrag.Value.Start();
            }
        }

        public System.Collections.Generic.List<string> Lesen()
        {
            var result = new System.Collections.Generic.List<string>();

            foreach (var eintrag in dateneinträge)
            {
                eintrag.Value.Lesen();
                if (eintrag.Value.aenderung)
                {
                    result.Add(eintrag.Key);
                }
            }

            return result;
        }

        public bool AenderungVorhanden()
        {
            foreach (var eintrag in dateneinträge)
                if (eintrag.Value.aenderung) return true;

            return false;
        }

        public void AenderungenZuruecksetzen()
        {
            foreach (var eintrag in dateneinträge)
                if (eintrag.Value.aenderung) eintrag.Value.aenderung = false;
        }

        public void AenderungenZuruecksetzen(System.Collections.Generic.List<string> liste)
        {
            foreach (var eintrag in liste)
                if (dateneinträge[eintrag].aenderung)dateneinträge[eintrag].aenderung = false;
        }

        public bool Lesen(string Identifikation)
        {
            if (this.dateneinträge.ContainsKey(Identifikation))
            {
                var eintrag = this.dateneinträge[Identifikation];
                eintrag.Lesen();
                return eintrag.aenderung;
            }
            return false;
        }

        public void Schreiben()
        {
            foreach (var eintrag in dateneinträge)
            {
                eintrag.Value.Schreiben();
            }
        }

        public void Schreiben(string Identifikation)
        {
            if (this.dateneinträge.ContainsKey(Identifikation))
            {
                var eintrag = this.dateneinträge[Identifikation];
                eintrag.Schreiben();
            }
        }
    }
}