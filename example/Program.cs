using System;

namespace example
{
    class Program
    {
        static void Main(string[] args)
        {
            int state_work = Int32.Parse(args[0]), state_target = Int32.Parse(args[1]);
            //int variables = Int32.Parse(args[2]);
            string model = args[2];

            var state = new library.var_int(0, "/media/ramdisk/state");
            var rr_ticks = new library.var_int(0, "/media/ramdisk/rr_ticks");
            //var val = new library.var_int(0, "/media/ramdisk/process/val");
            //var val2 = new library.var_int(0, "/media/ramdisk/process/val2");

            var rnd = new Random();
            var curr_ticks = System.DateTime.Now.Ticks;
            //val.value = rnd.Next();
            
            var structure = new library.Datastructure();
            structure.AddDatamodelFromFile(model);

            var vals = new System.Collections.Generic.List<library.var_int>();
            var vals_return = new System.Collections.Generic.List<library.var_int>();
            var waiting = new System.Collections.Generic.List<int>();

            var dm_application = new library.Datamodel("example");
            dm_application.AddEvaluationLead(100);
            dm_application.AddEvaluationSet(100, "example_");
            structure.AddDatamodel(dm_application);
            /*for(int i = 0; i < variables; ++i)
            {
                vals.Add(new library.var_int(0, "/media/ramdisk/process/val" + i.ToString()));
                vals_return.Add(new library.var_int(0, "/media/ramdisk/process/val_return" + i.ToString()));
            }

            while(true)
            {
                state.WertLesen();

                if (state == state_work)
                {
                    state.value = state_target;

                    if (waiting.Count > 0)
                    {
                        for (int i = 0; i < waiting.Count; ++i)
                        {
                            vals_return[waiting[i]].WertLesen();

                            if (vals_return[waiting[i]].aenderung)
                            {
                                vals_return[waiting[i]].aenderung = false;

                                if (vals_return[waiting[i]] == vals[waiting[i]])
                                {
                                    waiting.RemoveAt(i);
                                    --i;

                                    if (waiting.Count == 0)
                                    {
                                        var curr_ticks_new = System.DateTime.Now.Ticks;
                                        rr_ticks.value = (int)(curr_ticks_new - curr_ticks);
                                        rr_ticks.WertSchreiben();
                                        curr_ticks = curr_ticks_new;
                                    }
                                }
                            }
                        }
                    } else {
                        var tmp = rnd.Next();
                        for(int i = 0; i < variables-1; ++i)
                        {
                            waiting.Add(i);
                            vals[i].value = tmp;
                            vals[i].WertSchreiben();
                        }
                    }

                    state.WertSchreiben();
                }
            }*/
        }
    }
}
