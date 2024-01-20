﻿using System;
using System.Linq;
using Common.Logging.Factory;

namespace isci
{
    public static class Helfer
    {
        private static System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<string>> Cache = null;

        public static System.Collections.Generic.List<string> ChangedFiles(string path)
        {
            var files_ = System.IO.Directory.GetFiles(path);
            var files = new System.Collections.Generic.List<string>(files_);

            if (Cache.ContainsKey(path))
            {
                var firstNotSecond = files.Except(Cache[path]).ToList();
                var secondNotFirst = Cache[path].Except(files).ToList();

                if (firstNotSecond.Count > 0) {
                    Cache[path].AddRange(firstNotSecond);
                }

                foreach (var s in secondNotFirst)
                {
                    Cache[path].Remove(s);
                }
            } else {
                Cache.Add(path, files);
            }
            return Cache[path];
        }

        public static void OrdnerPruefenErstellen(string pfad)
        {
            if (!System.IO.Directory.Exists(pfad)) System.IO.Directory.CreateDirectory(pfad);
        }
    }

    public static class Logger
    {
        public enum Qualität
        {
            INFO,
            ERROR
        }

        private static string pfad;
        private static string identifikation;

        public static void Konfigurieren(Allgemein.Parameter parameter)
        {
            identifikation = parameter.Identifikation;
            pfad = parameter.OrdnerLogs + "/" + identifikation + ".log";
        }

        public static void Loggen(Qualität qualität, string nachricht)
        {
            var timeStamp = DateTime.Now.ToString("O");
            var eintrag = "{" + $"\"Zeitstempel\":\"{timeStamp}\",\"Qualität\":\"{qualität.ToString()}\",\"Inhalt\":\"{nachricht}\",\"Modulinstanz\":\"{identifikation}\"" + "}";

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(pfad, true))
            {
                writer.WriteLine(eintrag);
            }
        }
    }
}