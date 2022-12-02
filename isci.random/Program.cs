using System;
using System.Linq;

namespace random
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfiguration = new library.Konfiguration("konfiguration.json");
            
            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0]);

            var dm = new library.Datamodel(konfiguration.Identifikation);
            var random = new library.var_int(0, "random");
            var cycle = new library.var_int(0, "cycle");
            dm.Datafields.Add(random);
            dm.Datafields.Add(cycle);
            System.IO.File.WriteAllText(konfiguration.OrdnerDatenmodelle[0] + "/" + konfiguration.Identifikation + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(dm));

            structure.AddDatamodel(dm);
            structure.Start();

            var Zustand = new library.var_int(0, "Zustand", konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0] + "/Zustand");
            Zustand.Start();

            var rnd = new Random();

            var timestamp = DateTime.Now;

            while(true)
            {
                Zustand.WertLesen();
                var erfüllteTransitionen = konfiguration.Zustandsbereiche.Where(a => a.Arbeitszustand == (System.Int32)Zustand.value);
                if (erfüllteTransitionen.Count<library.Zustandsbereich>() > 0)
                {
                    structure.UpdateImage();

                    var tmp = DateTime.Now;
                    cycle.value = (System.Int32)(tmp - timestamp).TotalMilliseconds;
                    timestamp = tmp;
                    random.value = rnd.Next();

                    structure.PublishImage();
                    
                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}