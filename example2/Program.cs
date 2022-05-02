using System;

namespace example2
{
    class Program
    {
        static void Main(string[] args)
        {
            /*var state =  new library.var_int(0, "/media/ramdisk/state");
            var val = new library.var_int(0, "/media/ramdisk/process/val");
            var val2 = new library.var_int(0, "/media/ramdisk/process/val2");*/

            int state_work = Int32.Parse(args[0]), state_target = Int32.Parse(args[1]);
            int variables = Int32.Parse(args[2]);

            var state = new library.var_int(0, "/media/ramdisk/state");

            var val_combination = new System.Collections.Generic.Dictionary<library.var_int, library.var_int>();
            for(int i = 0; i < variables; ++i)
            {
                val_combination.Add(new library.var_int(0, "/media/ramdisk/process/val" + i.ToString()),
                                    new library.var_int(0, "/media/ramdisk/process/val_return" + i.ToString()));
            }

            while(true)
            {
                state.WertLesen();

                if (state == state_work)
                {
                    foreach (var val_pair in val_combination)
                    {
                        val_pair.Key.WertLesen();
                        if (val_pair.Key.aenderung)
                        {
                            val_pair.Key.aenderung = false;
                            val_pair.Value.value = val_pair.Key.value;
                            val_pair.Value.WertSchreiben();
                        }
                    }
                    /*val.WertLesen();
                    if (val.aenderung)
                    {
                        val.aenderung = false;
                        val2.value = val.value;
                        val2.WertSchreiben();
                    }*/

                    //++state.value;
                    state.value = state_target;
                    state.WertSchreiben();
                }
            }            
        }
    }
}
