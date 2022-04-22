using System;

namespace example
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new library.var_int(0, "/media/ramdisk/state");
            var rr_ticks = new library.var_int(0, "/media/ramdisk/rr_ticks");
            var val = new library.var_int(0, "/media/ramdisk/process/val");
            var val2 = new library.var_int(0, "/media/ramdisk/process/val2");

            var rnd = new Random();
            var curr_ticks = System.DateTime.Now.Ticks;
            val.value = rnd.Next();

            while(true)
            {
                state.WertLesen();

                if (state == 1)
                {
                    val2.WertLesen();
                    if (val2.aenderung)
                    {
                        val2.aenderung = false;
                        if (val == val2)
                        {
                            var curr_ticks_new = System.DateTime.Now.Ticks;
                            rr_ticks.value = (int)(curr_ticks_new - curr_ticks);
                            rr_ticks.WertSchreiben();
                            curr_ticks = curr_ticks_new;
                            val.value = rnd.Next();
                            val.WertSchreiben();
                        }
                    }

                    ++state.value;
                    state.WertSchreiben();
                }
            }
        }
    }
}
