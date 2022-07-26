using System;
using System.Linq;

namespace MODULBASIS {

    class Program
    {
        static void Main(string[] args)
        {
            var konfiguration = new library.Konfiguration("konfiguration.json");
            var structure = new library.Datastructure(konfiguration.OrdnerDatenstruktur);
            var dm = new library.Datamodel(konfiguration.Identifikation);

            System.IO.File.WriteAllText(konfiguration.OrdnerDatenmodelle + "/" + konfiguration.Identifikation + ".json",
                                        Newtonsoft.Json.JsonConvert.SerializeObject(dm));

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

                    foreach (var entry in structure.Datafields)
                    {
                        if (entry.Value.aenderung)
                        {
                            entry.Value.aenderung = false;
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