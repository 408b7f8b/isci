using System;
using System.Linq;

namespace link
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfiguration = new library.Konfiguration("konfiguration.json");

            var beschreibung = new Beschreibung.Modul();
            beschreibung.Identifikation = konfiguration.Identifikation;
            beschreibung.Name = "Link Ressource " + konfiguration.Identifikation;
            beschreibung.Beschreibung = "Modul zur Verknüpfung von Dateneinträgen";
            beschreibung.Typidentifikation = "isci.link";
            beschreibung.Datenfelder = new library.FieldList();
            beschreibung.Ereignisse = new System.Collections.Generic.List<library.Ereignis>();
            beschreibung.Funktionen = new System.Collections.Generic.List<library.Funktion>();
            beschreibung.Speichern(konfiguration.OrdnerBeschreibungen + "/" + konfiguration.Identifikation + ".json");
            
            var structure = new library.Datastructure(konfiguration.OrdnerDatenstruktur);

            var dm = new library.Datamodel(konfiguration.Identifikation);

            System.IO.File.WriteAllText(konfiguration.OrdnerDatenmodelle + "/" + konfiguration.Identifikation + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(dm));

            structure.AddDatamodel(dm);
            structure.AddDataModelsFromDirectory(konfiguration.OrdnerDatenmodelle, konfiguration.Identifikation);

            structure.GenerateLinks();

            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstruktur + "/Zustand");
            Zustand.Start();

            while(true)
            {
                Zustand.WertLesen();
                var erfüllteTransitionen = konfiguration.Zustandsbereiche.Where(a => a.Arbeitszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<library.Zustandsbereich>() > 0)
                {
                    structure.UpdateImage();

                    foreach (var entry in structure.LinksActive)
                    {
                        if (entry.Key.aenderung)
                        {
                            foreach (var subentry in entry.Value)
                            {
                                Console.WriteLine(System.DateTime.Now.ToString("O") + ": " + entry.Key.Identifikation + " --> " + subentry.Identifikation);
                                subentry.value = entry.Key.value;
                                entry.Key.aenderung = false;
                                subentry.WertSchreiben();
                            }
                        }
                    }

                    //structure.PublishImage();
                    
                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}