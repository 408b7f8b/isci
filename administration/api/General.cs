using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api
{
    public static class General
    {
        public static string FilesInPath(string pfad)
        {
            var list = System.IO.Directory.GetFiles(pfad, "*", System.IO.SearchOption.AllDirectories);

            string ret = "[";

            foreach (var file in list)
            {
                ret += ("\"" + file + "\"\n");
            }

            ret += "]";

            return ret;
        }

        public static string FoldersInPath(string pfad)
        {
            var list = System.IO.Directory.GetDirectories(pfad, "*", System.IO.SearchOption.TopDirectoryOnly);

            string ret = "[";

            foreach (var folder in list)
            {
                ret += ("\"" + folder + "\"\n");
            }

            ret += "]";

            return ret;
        }

        public static void Action(string filename, string arguments)
        {
            var process = new System.Diagnostics.Process();
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.FileName = filename;           
            startInfo.Arguments = arguments;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}