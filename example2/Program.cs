using System;

namespace example2
{
    class Program
    {
        static void Main(string[] args)
        {
            var state =  new library.var_int(0, "/media/ramdisk/state");
            var val = new library.var_int(0, "/media/ramdisk/process/val");
            var val2 = new library.var_int(0, "/media/ramdisk/process/val2");

            while(true)
            {
                state.WertLesen();

                if (state == 1)
                {
                    val.WertLesen();
                    if (val.aenderung)
                    {
                        val.aenderung = false;
                        val2.value = val.value;
                        val2.WertSchreiben();
                    }
                }

                ++state.value;
                state.WertSchreiben();
            }            
        }
    }
}
