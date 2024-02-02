﻿using System;
using System.Linq;
using Common.Logging.Factory;

/*
#if X64
#if WINDOWS
[System.Runtime.InteropServices.DllImport("lib_wait_us-win-x64.dll")]
#else
[System.Runtime.InteropServices.DllImport("lib_wait_us-linux-x64.so")]
#endif
public static extern void WaitForMicroseconds(uint microseconds);

#elif X86
#if WINDOWS
[System.Runtime.InteropServices.DllImport("lib_wait_us-win-x86.dll")]
#else
[System.Runtime.InteropServices.DllImport("lib_wait_us-linux-x86.so")]
#endif
public static extern void WaitForMicroseconds(uint microseconds);

#elif ARM
#if WINDOWS
[System.Runtime.InteropServices.DllImport("lib_wait_us-win-arm.dll")]
#else
[System.Runtime.InteropServices.DllImport("lib_wait_us-linux-arm.so")]
#endif
public static extern void WaitForMicroseconds(uint microseconds);

#elif ANYCPU
#if WINDOWS
[System.Runtime.InteropServices.DllImport("lib_wait_us-win-arm.dll")]
#else
[System.Runtime.InteropServices.DllImport("lib_wait_us-linux-arm.so")]
#endif
public static extern void WaitForMicroseconds(uint microseconds);

#endif*/

namespace isci
{
    public static class Helfer
    {
        [System.Runtime.InteropServices.DllImport("lib_wait_us.so")]
        public static extern void SleepForMicroseconds(uint microseconds);
        public static uint OptimiereSleepForMicroseconds(uint ZieldauerInUs, uint maximumIterationen = 100, double zulFehler = 0.01)
        {
            int iteration = 0;
            double adjustmentFactor = 1.0;
            double errorPercentage = 0.0;

            long ticksPerSecond = System.Diagnostics.Stopwatch.Frequency;
            var fuer1Us = (double)(TimeSpan.TicksPerMillisecond / 1000);

            uint adjustedMicroseconds;

            do
            {
                // Calculate adjusted sleep duration based on the adjustment factor
                adjustedMicroseconds = (uint)(ZieldauerInUs * adjustmentFactor);

                // Get the start time
                var start = System.DateTime.Now.Ticks;

                // Sleep for the adjusted duration
                isci.Helfer.SleepForMicroseconds(adjustedMicroseconds);

                // Get the actual sleep duration
                var end = System.DateTime.Now.Ticks;
                var actualDuration = end - start;

                // Calculate the error percentage
                errorPercentage = (actualDuration - ZieldauerInUs * fuer1Us) / (ZieldauerInUs * fuer1Us);

                // Adjust the factor based on the error percentage
                adjustmentFactor = 1.0 - (errorPercentage * zulFehler);

                // Increment the iteration count
                iteration++;
            } while (Math.Abs(errorPercentage) > zulFehler && iteration < maximumIterationen);

            return adjustedMicroseconds;
        }

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
        private static string identifikation = "";
        
        public static void temporärKonfigurieren(string identifikation)
        {
            Logger.identifikation = identifikation;
        }

        public static void Konfigurieren(Allgemein.Parameter parameter)
        {
            identifikation = parameter.Identifikation;
        }

        public static void Loggen(Qualität qualität, string nachricht)
        {
            var timeStamp = DateTime.Now.ToString("O");
            var eintrag = "{" + $"\"Zeitstempel\":\"{timeStamp}\",\"Qualität\":\"{qualität.ToString()}\",\"Inhalt\":\"{nachricht}\",\"Modulinstanz\":\"{identifikation}\"" + "}";

            System.Console.WriteLine(eintrag);
        }
    }
}