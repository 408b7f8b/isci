using System;
using System.Linq;

namespace isci.Daten
{
    /// <summary>
    /// Datenstrukturklasse zur Abbildung einer Dateisystem-Datenstruktur in einem programmatischen Objekt.
    /// </summary>
    public class Datenstruktur
    {
        /// <summary>
        /// Statische Methode zur Validierung und Korrektur einer Dateneintrag-Identifikation nach dem Muster 'anwendung.datenmodell.spezifischeIdentifikationDateneintrag'.
        /// </summary>
        /// <param name="identifikation">Aktuelle Identifikation des Dateneintrags.</param>
        /// <param name="anwendung">Korrekte Identifikation der Anwendung.</param>
        /// <param name="datenmodell">Korrekte Identifikation des Datenmodells.</param>
        /// <returns>Korrekter Identifikations-String.</returns>
        private static string validiereIdentifikation(string identifikation, string anwendung, string datenmodell)
        {
            string correctedString = identifikation;

            if (!correctedString.StartsWith($"{anwendung}.{datenmodell}."))
            {
                if (correctedString.Contains('.'))
                {
                    correctedString = $"{anwendung}.{datenmodell}.{correctedString.Substring(correctedString.LastIndexOf('.'))}";
                } else {
                    correctedString = $"{anwendung}.{datenmodell}.{correctedString}";
                }
            }

            return correctedString;
        }

        /// <summary>
        /// Statische Methode zur Validierung und Korrektur der Identifikation eines Dateneintrags nach dem Muster 'anwendung.datenmodell.spezifischeIdentifikationDateneintrag'.
        /// </summary>
        /// <param name="eintrag">Zu überprüfender und korrigierender Dateneintrag.</param>
        /// <param name="anwendung">Korrekte Identifikation der Anwendung.</param>
        /// <param name="datenmodell">Korrekte Identifikation des Datenmodells.</param>
        private static void validiereIdentifikationEintrag(Dateneintrag eintrag, string anwendung, string datenmodell)
        {
            eintrag.Identifikation = validiereIdentifikation(eintrag.Identifikation, anwendung, datenmodell);
        }

        public System.Collections.Generic.List<string> datenmodelle;
        public KarteDateneintraege dateneinträge;
        public VerweislisteDateneintraege verweise;
        public VerweislisteDateneintraegeAktiv verweiseAktiv;        
        public System.Collections.Generic.List<string> nichtVerteilen;
        private string OrdnerDatenstrukturen;
        private string Modulidentifikation;
        private string Automatisierungssystem;
        public dtZustand Zustand;

        /// <summary>
        /// Konstruktor zur Erstellung eines Datenstrukturobjekts. Standardkonstruktor.
        /// </summary>
        /// <param name="parameter">Parameterobjekt, mit dem die nutzende Modulinstanz konfiguriert wird.</param>
        /// <returns>Das erstellte Datenstrukturobjekt.</returns>
        public Datenstruktur(isci.Allgemein.Parameter parameter)
        {
            datenmodelle = new System.Collections.Generic.List<string>();
            dateneinträge = new KarteDateneintraege();
            verweise = new VerweislisteDateneintraege();
            verweiseAktiv = new VerweislisteDateneintraegeAktiv();
            nichtVerteilen = new System.Collections.Generic.List<string>();
            if (parameter.OrdnerDatenstrukturen.EndsWith(parameter.Anwendung))
            {
                this.OrdnerDatenstrukturen = parameter.OrdnerDatenstrukturen;
            } else {
                this.OrdnerDatenstrukturen = parameter.OrdnerDatenstrukturen + "/" + parameter.Anwendung;
            }            
            this.Modulidentifikation = parameter.Identifikation;
            this.Automatisierungssystem = parameter.Anwendung;
            this.ZustandsdateneintragAnlegen(parameter);
            Logger.Loggen(Logger.Qualität.INFO, "Datenstruktur erstellt.");
        }

        /// <summary>
        /// Konstruktor zur Erstellung eines Datenstrukturobjekts.
        /// </summary>
        /// <param name="OrdnerDatenstrukturen">OrdnerDatenstrukturen zur Datenstruktur auf dem Dateisystem.</param>
        /// <param name="identifikation">Identifikation der betreibenden Modulinstanz.</param>
        /// <param name="automatisierungssystem">Identifikation des übergeordneten Automatisierungssystems.</param>
        /// <returns>Das erstellte Datenstrukturobjekt.</returns>
        public Datenstruktur(string OrdnerDatenstrukturen, string identifikation, string automatisierungssystem)
        {
            datenmodelle = new System.Collections.Generic.List<string>();
            dateneinträge = new KarteDateneintraege();
            verweise = new VerweislisteDateneintraege();
            verweiseAktiv = new VerweislisteDateneintraegeAktiv();
            nichtVerteilen = new System.Collections.Generic.List<string>();
            this.OrdnerDatenstrukturen = OrdnerDatenstrukturen;
            this.Modulidentifikation = identifikation;
            this.Automatisierungssystem = automatisierungssystem;
            this.ZustandsdateneintragAnlegen(OrdnerDatenstrukturen, automatisierungssystem);
            Logger.Loggen(Logger.Qualität.INFO, "Datenstruktur erstellt.");
        }

        /// <summary>
        /// Einhängen eines Datenmodells in die Datenstruktur.
        /// </summary>
        /// <param name="datenmodell">Einzuhängendes Datenmodell.</param>
        public void DatenmodellEinhängen(Datenmodell datenmodell)
        {
            try //diese Try-catch-Schleife ist nicht logisch aufgebaut. Ich brauche Debug-Output, um zu erkennen, was schiefläuft und 
            {
                foreach (var eintrag in datenmodell.Dateneinträge)
                {
                    validiereIdentifikationEintrag(eintrag, this.Automatisierungssystem, datenmodell.Identifikation);
                    eintrag.path = OrdnerDatenstrukturen + "/" + eintrag.Identifikation;
                    dateneinträge.Add(eintrag.Identifikation, eintrag);
                }
                foreach (var eintrag in datenmodell.Links)
                {
                    var Links_ = new ListeDateneintraege();
                    var key = eintrag.Key;
                    var value = new System.Collections.Generic.List<string>();
                    foreach (var untereintrag in eintrag.Value)
                    {
                        var eintrag_ = "";
                        value.Add(eintrag_);
                        if (!nichtVerteilen.Contains(eintrag_)) nichtVerteilen.Add(eintrag_);
                    }
                    verweise.Add(key, value);
                }
                datenmodelle.Add(datenmodell.Identifikation);
            } catch (Exception)
            {
                
            }
        }

        /// <summary>
        /// Einhängen eines bestimmten Datenmodells über seine Datei.
        /// </summary>
        /// <param name="OrdnerDatenstrukturen">OrdnerDatenstrukturen zur Datei des Datenmodells.</param>
        public void DatenmodellEinhängenAusDatei(string OrdnerDatenstrukturen)
        {
            Logger.Loggen(Logger.Qualität.INFO, $"Datenstruktur: Datenmodell {OrdnerDatenstrukturen} wird eingehängt.");
            var dm_ = Datenmodell.AusDatei(OrdnerDatenstrukturen);
            DatenmodellEinhängen(dm_);
        }

        /// <summary>
        /// Alle als Dateien abgelegte Datenmodelle aus dem internen OrdnerDatenstrukturen-Ordner einhängen.
        /// </summary>
        /*public void DatenmodelleEinhängen()
        {
            this.DatenmodelleEinhängenAusOrdner(this.OrdnerDatenstrukturen, this.identifikation);
        }*/

        /// <summary>
        /// Alle als Dateien abgelegte Datenmodelle aus einem Ordner einhängen.
        /// </summary>
        /// <param name="param1">OrdnerDatenstrukturen, der verarbeitet werden soll. Unterordner werden nicht beachtet.</param>
        /// <param name="excludeown">Angabe der Modulinstanz-Identifikation, falls ein Datenmodell mit dieser Identifikation ignoriert werden soll.</param>
        public void DatenmodelleEinhängenAusOrdner(string OrdnerDatenstrukturene, string excludeown = "")
        {
            if (OrdnerDatenstrukturene == null)
            {
                Console.WriteLine("DatenmodelleEinhängenAusOrdner: OrdnerDatenstrukturene ist NULL");
                return;
            }
            
            var dms_ = System.IO.Directory.GetFiles(OrdnerDatenstrukturene);
            foreach (var dm_ in dms_)
            {
                if (excludeown != "")
                {
                    if (dm_.Contains(excludeown)) continue;
                }                
                DatenmodellEinhängenAusDatei(dm_);
            }
        }

        /// <summary>
        /// Alle als Dateien abgelegte Datenmodelle aus mehreren Ordnern einhängen.
        /// </summary>
        /// <param name="OrdnerDatenstrukturene">Liste, der OrdnerDatenstrukturene, die verarbeitet werden sollen. Unterordner werden nicht beachtet.</param>
        /// <param name="excludeown">Angabe der Modulinstanz-Identifikation, falls ein Datenmodell mit dieser Identifikation ignoriert werden soll.</param>
        public void DatenmodelleEinhängenAusOrdnern(string[] OrdnerDatenstrukturene, string excludeown = "")
        {
            foreach (var OrdnerDatenstrukturen in OrdnerDatenstrukturene)
            {
                DatenmodelleEinhängenAusOrdner(OrdnerDatenstrukturen, excludeown);
            }
        }

        /// <summary>
        /// Laufzeitverweise zwischen Dateneinträgen aus dem Datenmodellangaben erzeugen.
        /// </summary>
        public void VerweiseErzeugen()
        {
            Logger.Loggen(Logger.Qualität.INFO, "Datenstruktur: Verweise werden erzeugt.");
            foreach (var eintrag in verweise)
            {
                foreach (var subentry in eintrag.Value)
                {
                    try {
                        verweiseAktiv.Add(dateneinträge[eintrag.Key], dateneinträge[subentry]);
                    } catch (System.Exception e) {
                        Logger.Loggen(Logger.Qualität.ERROR, "Datenstruktur: Fehler beim Erzeugen der Verweise: " + e.Message);
                    }
                }
            }
        }

        public void ZustandsdateneintragAnlegen(isci.Allgemein.Parameter parameter)
        {
            ZustandsdateneintragAnlegen(parameter.OrdnerDatenstrukturen, parameter.Anwendung);
        }

        public void ZustandsdateneintragAnlegen(string OrdnerDatenstrukturen, string Anwendung)
        {
            Zustand = new dtZustand(OrdnerDatenstrukturen + "/" + Anwendung);
            this.dateneinträge.Add(Zustand.Identifikation, Zustand);
        }

        /// <summary>
        /// Verbindung der programmatischen Datenstruktur mit der Datenstruktur auf dem Dateisystem herstellen.
        /// </summary>
        public void Start()
        {
            Logger.Loggen(Logger.Qualität.INFO, "Datenstruktur wird gestartet.");
            foreach (var eintrag in dateneinträge)
            {
                eintrag.Value.Start();
            }
        }

        /// <summary>
        /// Lesen aller Dateneinträge vom Dateisystem.
        /// </summary>
        /// <returns>Liste der Identifikationen der Dateneinträge mit Werteänderung.</returns>
        public System.Collections.Generic.List<string> Lesen()
        {
            var result = new System.Collections.Generic.List<string>();

            foreach (var eintrag in dateneinträge)
            {
                eintrag.Value.WertAusSpeicherLesen();
                if (eintrag.Value.aenderungExtern)
                {
                    result.Add(eintrag.Key);
                }
            }

            return result;
        }

        /// <summary>
        /// Lesen eines bestimmten Dateneintrags vom Dateisystem anhand seiner Identifikation.
        /// </summary>
        /// <param name="Identifikation">Identifikation des zu lesenden Dateneintrags.</param>
        /// <returns>True, wenn eine Werteänderung bei ihm vorliegt oder der Dateneintrag nicht existiert, ansonsten false.</returns>
        public bool Lesen(string Identifikation)
        {
            if (this.dateneinträge.ContainsKey(Identifikation))
            {
                var eintrag = this.dateneinträge[Identifikation];
                eintrag.WertAusSpeicherLesen();
                return eintrag.aenderungExtern;
            }
            return false;
        }

        /// <summary>
        /// Abfrage ob Änderung bei einem oder mehreren Dateneinträgen vorhanden sind.
        /// </summary>
        /// <returns>Rückgabe true, wenn einer oder mehrere Dateneinträge eine gesetzte Änderungsmarkierung hat, ansonsten false.</returns>
        public bool AenderungVorhanden()
        {
            foreach (var eintrag in dateneinträge)
                if (eintrag.Value.aenderungExtern) return true;

            return false;
        }

        /// <summary>
        /// Änderungsmarkierungen aller Dateneinträge zurücksetzen.
        /// </summary>
        public void AenderungenZuruecksetzen()
        {
            foreach (var eintrag in dateneinträge)
                if (eintrag.Value.aenderungExtern) eintrag.Value.aenderungExtern = false;
        }

        /// <summary>
        /// Änderungsmarkierungen zurücksetzen anhand einer gegebenen Liste
        /// </summary>
        /// <param name="liste">Liste der Identifikationen der Dateneinträge, deren Änderungsmarkierungen zurückgesetzt werden sollen.</param>
        public void AenderungenZuruecksetzen(System.Collections.Generic.List<string> liste)
        {
            foreach (var eintrag in liste)
                if (dateneinträge[eintrag].aenderungExtern) dateneinträge[eintrag].aenderungExtern = false;
        }

        /// <summary>
        /// Aktuelle Werte aller Dateneinträge auf das Dateisystem übernehmen.
        /// </summary>
        public void Schreiben()
        {
            foreach (var eintrag in dateneinträge)
            {
                if (eintrag.Value.aenderungIntern)
                {
                    eintrag.Value.WertInSpeicherSchreiben();
                }
                eintrag.Value.aenderungExtern = false;
            }
        }

        /// <summary>
        /// Aktuellen Wert eines bestimmten Dateneintrags auf das Dateisystem übernehmen.
        /// </summary>
        /// <param name="Identifikation">Identifikation des Dateneintrags als string.</param>
        public void Schreiben(string Identifikation)
        {
            if (this.dateneinträge.ContainsKey(Identifikation))
            {
                var eintrag = this.dateneinträge[Identifikation];
                eintrag.WertInSpeicherSchreiben();
            }
        }

        public Dateneintrag this[string key]
        {
            get
            {
                Dateneintrag ret = null;
                ret = this.dateneinträge[key];
                return ret;
            }
        }
    }
}