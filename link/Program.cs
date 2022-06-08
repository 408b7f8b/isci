using System;
using System.Linq;

namespace link
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfiguration = new library.Konfiguration("konfiguration.json");
            
            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0]);

            var dm = new library.Datamodel(konfiguration.Identifikation);

            System.IO.File.WriteAllText(konfiguration.OrdnerDatenmodelle[0] + "/" + konfiguration.Identifikation + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(dm));

            structure.AddDatamodel(dm);
            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0] + "/Zustand");
            Zustand.Start();

            while(true)
            {
                Zustand.WertLesen();
                var erfüllteTransitionen = konfiguration.Zustandsbereiche.Where(a => a.Arbeitszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<library.Zustandsbereich>() > 0)
                {
                    structure.UpdateImage();

                    foreach (var entry in structure.Links)
                    {
                        if (entry.Key.aenderung)
                        {
                            foreach (var subentry in entry.Value)
                            {
                                subentry.value = entry.Key.value;
                            }
                        }
                    }

                    structure.PublishImage();
                    
                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}