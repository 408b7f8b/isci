using System;

namespace tracker
{
    class Program
    {
        static void Main(string[] args)
        {
            var rr_ticks = new library.var_int(0, "/media/ramdisk/rr_ticks");
            var cycletime = new library.var_int(0, "/media/ramdisk/cycletime");

            int count_rr = 0, count_cycle = 0;
            int length = int.Parse(args[0]);
            var rr_ticks_count = new int[length];
            var cycletime_count = new int[length];

            while(true)
            {
                rr_ticks.WertLesen();
                if (rr_ticks.aenderung)
                {
                    rr_ticks.aenderung = false;
                    rr_ticks_count[count_rr] = rr_ticks.value;
                    ++count_rr;
                }
                if (count_rr == length)
                {
                    var file = "/media/ramdisk/rr_ticks_" + System.DateTime.Now.ToString("O");
                    var appender = System.IO.File.AppendText(file);

                    foreach (var rr in rr_ticks_count)
                    {
                        appender.WriteLine(rr.ToString());
                    }
                    appender.Close();
                    count_rr = 0;
                }
                
                cycletime.WertLesen();
                if (cycletime.aenderung)
                {
                    cycletime.aenderung = false;
                    cycletime_count[count_cycle] = cycletime.value;
                    ++count_cycle;
                }
                if (count_cycle == length)
                {
                    var file = "/media/ramdisk/cycletime_" + System.DateTime.Now.ToString("O");
                    var appender = System.IO.File.AppendText(file);

                    foreach (var cycle in cycletime_count)
                    {
                        appender.WriteLine(cycle.ToString());
                    }
                    appender.Close();
                    count_cycle = 0;
                }
            }
        }
    }
}
