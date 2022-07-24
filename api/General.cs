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
            var list = Newtonsoft.Json.JsonConvert.SerializeObject(FilesInPathList(pfad));

            return list;
        }

        public static List<string> FilesInPathList(string pfad)
        {
            var list = System.IO.Directory.GetFiles(pfad, "*", System.IO.SearchOption.TopDirectoryOnly).ToList<string>();

            return list;
        }

        public static string FoldersInPath(string pfad)
        {
            var list = Newtonsoft.Json.JsonConvert.SerializeObject(FoldersInPathList(pfad));

            return list;
        }

        public static List<string> FoldersInPathList(string pfad)
        {
            var list = System.IO.Directory.GetDirectories(pfad, "*", System.IO.SearchOption.TopDirectoryOnly).ToList<string>();

            return list;
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