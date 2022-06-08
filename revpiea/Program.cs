using System;
using System.Linq;

namespace revpiea
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfiguration = new library.Konfiguration("konfiguration.json");
            
            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0]);

            RevPiZugriff.SystemkonfigurationLesen();
            RevPiZugriff.EinUndAusgängeAufstellen();
            RevPiZugriff.control.Open();

            var dm = new library.Datamodel(konfiguration.Identifikation);

            var Ausgaenge = new System.Collections.Generic.Dictionary<library.var_int, ioObjekt>();
            var Eingaenge = new System.Collections.Generic.Dictionary<ioObjekt, library.var_int>();

            foreach (var eintrag_ in RevPiZugriff.Eingänge)
            {
                var eintrag = eintrag_.Value.EintragErstellen("");
                dm.Datafields.Add(eintrag);
                Eingaenge.Add(eintrag_.Value, eintrag);
            }

            foreach (var eintrag_ in RevPiZugriff.Ausgänge)
            {
                var eintrag = eintrag_.Value.EintragErstellen("");
                dm.Datafields.Add(eintrag);
                Ausgaenge.Add(eintrag, eintrag_.Value);
            }

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
                    if (erfüllteTransitionen.ElementAt(0) == konfiguration.Zustandsbereiche[0])
                    {
                        foreach (var Eingang in Eingaenge)
                        {
                            object o = null;
                            if (Eingang.Key.Zustandlesen(out o))
                            {
                                switch (Eingang.Key.typ)
                                {
                                    case ioObjekt.Typ.BOOL: Eingang.Value.value = ((bool)o ? 1 : 0); break;
                                    case ioObjekt.Typ.BYTE: Eingang.Value.value = (int)((byte)o); break;
                                    case ioObjekt.Typ.WORD: Eingang.Value.value = o; break;
                                    case ioObjekt.Typ.INT: Eingang.Value.value = o; break;
                                    default: continue;
                                }
                            
                                Eingang.Value.WertSchreiben();
                            }
                        }
                        structure.PublishImage();
                    } else {
                        structure.UpdateImage();
                        foreach (var Ausgang in Ausgaenge)
                        {
                            Ausgang.Key.WertLesen();
                            if (Ausgang.Key.aenderung)
                            {
                                switch (Ausgang.Value.typ)
                                {
                                    case ioObjekt.Typ.BOOL: Ausgang.Value.Zustandschreiben((System.Int32)Ausgang.Key.value == 1 ? true : false); break;
                                    case ioObjekt.Typ.BYTE: Ausgang.Value.Zustandschreiben((byte)Ausgang.Key.value); break;
                                    case ioObjekt.Typ.WORD: Ausgang.Value.Zustandschreiben((short)Ausgang.Key.value); break;
                                    case ioObjekt.Typ.INT: Ausgang.Value.Zustandschreiben((System.Int32)Ausgang.Key.value); break;
                                    default: continue;
                                }

                                Ausgang.Key.aenderung = false;
                            }
                        }                        
                    }                    
                    
                    Zustand.value = erfüllteTransitionen.First<library.Zustandsbereich>().Nachfolgezustand;
                    Zustand.WertSchreiben();
                }
            }
        }
    }
}