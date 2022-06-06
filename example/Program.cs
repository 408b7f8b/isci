using System;
using System.Linq;

namespace evaluation
{
    class Program
    {
        static void Main(string[] args)
        {
            var konfiguration = new library.Konfiguration("konfiguration.json");
            
            var structure = new library.Datastructure(konfiguration.OrdnerDatenstrukturen[0] + "/" + konfiguration.Anwendungen[0]);

            var evaluation = new library.Datamodel("Evaluation");

            var ressourceList = Newtonsoft.Json.JsonConvert.DeserializeObject<library.RessourceList>(System.IO.File.ReadAllText(konfiguration.OrdnerAnwendungen[0] + "/Ressourcen"));

            var monitor_and_resp = new System.Collections.Generic.Dictionary<library.var_int, library.var_int>();

            var do_events = new System.Collections.Generic.Dictionary<library.var_int, library.var_int>();
            var event_resp = new System.Collections.Generic.Dictionary<library.var_int, library.var_int>();
            var resp_events = new System.Collections.Generic.Dictionary<library.var_int, library.var_int>();
            var event_worsttime = new System.Collections.Generic.Dictionary<library.var_int, library.var_int>();

            foreach (var entry in ressourceList)
            {
                if (entry == konfiguration.Ressource) continue;

                var monitor = new library.var_int(0, "Event_" + entry + "_" + konfiguration.Ressource);
                evaluation.Datafields.Add(monitor); //hier erhalten
                var resp = new library.var_int(0, "Resp_" + entry + "_" + konfiguration.Ressource);
                evaluation.Datafields.Add(resp); //hier beantworten
                monitor_and_resp.Add(monitor, resp);

                var doEvent = new library.var_int(0, "Do_" + konfiguration.Ressource + "_" + entry);
                evaluation.Datafields.Add(doEvent);
                var Event = new library.var_int(0, "Event_" + konfiguration.Ressource + "_" + entry);
                evaluation.Datafields.Add(Event);
                var eventResp = new library.var_int(0, "Resp_" + konfiguration.Ressource + "_" + entry);
                evaluation.Datafields.Add(eventResp);
                do_events.Add(doEvent, Event);
                event_resp.Add(Event, eventResp);
                resp_events.Add(eventResp, Event);

                var event_w = new library.var_int(0, "t_worst_" + konfiguration.Ressource + "_" + entry);
                evaluation.Datafields.Add(event_w); //mittelwert eigene messung
                event_worsttime.Add(Event, event_w);
            }

            structure.AddDatamodel(evaluation);
            structure.Start();

            System.IO.File.WriteAllText(konfiguration.OrdnerDatenmodelle[0] + "/Evaluation.json", Newtonsoft.Json.JsonConvert.SerializeObject(evaluation));

            var todo = new System.Collections.Generic.Dictionary<library.var_int, int>();
            var expected = new System.Collections.Generic.Dictionary<library.var_int, int>();
            var sent_timestamp = new System.Collections.Generic.Dictionary<library.var_int, long>();
            var to_send = new System.Collections.Generic.List<library.var_int>();
            var meas = new System.Collections.Generic.Dictionary<library.var_int, System.Collections.Generic.List<long>>();
            var to_eval = new System.Collections.Generic.List<library.var_int>();

            var rnd = new Random();

            while(true)
            {
                structure.UpdateImage();

                foreach (var entry in monitor_and_resp)
                {
                    if (entry.Key.aenderung)
                    {
                        entry.Key.aenderung = false;
                        entry.Value.value = entry.Key.value;
                    }
                }

                foreach (var entry in do_events)
                {
                    if (entry.Key.aenderung) //Eventanforderung
                    {
                        entry.Key.aenderung = false;
                        if (!todo.ContainsKey(entry.Value)) //ist das Event schon auf der Ausführen-Liste?
                        {
                            todo.Add(entry.Value, (System.Int32)entry.Key.value); //hinzufügen mit seiner Häufigkeit
                            meas.Add(entry.Value, new System.Collections.Generic.List<long>()); //
                            to_send.Add(entry.Value);
                        }
                    }
                }

                var exp_keys = expected.Keys.ToList<library.var_int>();
                for (int i = 0; i < exp_keys.Count; ++i)
                {
                    var entry = exp_keys[i];
                    if (entry.aenderung && (System.Int32)entry.value == expected[entry])
                    {
                        var curr_ticks_new = System.DateTime.Now.Ticks;
                        var triggering_event = resp_events[entry];
                        meas[triggering_event].Add(curr_ticks_new - sent_timestamp[triggering_event]);
                    }
                }
                
                if (to_send.Count > 0)
                {
                    var tmp = rnd.Next();
                    var curr_ticks_new = System.DateTime.Now.Ticks;

                    foreach (var entry in to_send)
                    {
                        entry.value = tmp;
                        if (sent_timestamp.ContainsKey(entry)) sent_timestamp[entry] = curr_ticks_new;
                        else sent_timestamp.Add(entry, curr_ticks_new);
                        expected.Add(event_resp[entry], tmp);
                        --todo[entry];
                        if (todo[entry] == 0)
                        {
                            to_eval.Add(entry);
                            todo.Remove(entry);                            
                        }
                    }
                    to_send.Clear();
                }                

                foreach (var entry in to_eval)
                {
                    long worst = 0;
                    foreach (var meas_ in meas[entry]) if (worst < meas_) worst = meas_;
                    event_worsttime[entry].value = (int)worst;
                    meas.Remove(entry);
                }
                to_eval.Clear();

                structure.PublishImage();
            }
        }
    }
}
