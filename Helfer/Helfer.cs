using System;
using System.Linq;
using System.Runtime.InteropServices;
using Common.Logging.Factory;

namespace isci
{
    public static class Helfer
    {
        [System.Runtime.InteropServices.DllImport("lib_wait_us_x86_64.a")]
        public static extern void SleepForMicrosecondsLinuxX64(uint microseconds);
        [System.Runtime.InteropServices.DllImport("lib_wait_us_arm64.a")]
        public static extern void SleepForMicrosecondsLinuxARM64(uint microseconds);
        [System.Runtime.InteropServices.DllImport("lib_wait_us-win-x64.dll")]
        public static extern void SleepForMicrosecondsWinX64(uint microseconds);
        public static void SleepAusweichfunktion(uint microseconds)
        {
            System.Threading.Thread.Sleep((int)microseconds/1000);
        }

        private static byte Architektur;
        public static void SetzeArchitektur()
        {
            var HW = System.Runtime.InteropServices.RuntimeInformation.OSArchitecture;
            
            if (HW == System.Runtime.InteropServices.Architecture.X64)
            {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                    Logger.Information("Plattform ist Win-X64.");
                    try {
                        Architektur = 0;
                        SleepForMicrosecondsWinX64(1);
                        return;
                    } catch (Exception e)
                    {
                        Logger.Fehler("Ausnahme bei Test der Bibliothek für Hilfsfunktionen: " + e.Message + ". Nutze architekturunabhängige Standardfunktionen.");
                    }
                } else if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    Logger.Information("Plattform ist Linux-X64.");
                    try {
                        Architektur = 1;
                        SleepForMicrosecondsLinuxX64(1);
                        return;
                    } catch (Exception e)
                    {
                        Logger.Fehler("Ausnahme bei Test der Bibliothek für Hilfsfunktionen: " + e.Message + ". Nutze architekturunabhängige Standardfunktionen.");
                    }
                }
            } else if( HW == Architecture.Arm64) {
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                    Logger.Information("Plattform ist Linux-Arm64.");
                    try {
                        Architektur = 2;
                        SleepForMicrosecondsLinuxARM64(1);
                        return;
                    } catch (Exception e)
                    {
                        Logger.Fehler("Ausnahme bei Test der Bibliothek für Hilfsfunktionen: " + e.Message + ". Nutze architekturunabhängige Standardfunktionen.");
                    }
                }
            }

            Logger.Warnung("Plattform ist unbekannt. Nutze architekturunabhängige Standardfunktionen.");

            Architektur = 255;
        }

        public static void SleepForMicroseconds(uint microseconds)
        {
            if (Architektur == 0) SleepForMicrosecondsWinX64(microseconds);
            else if (Architektur == 1) SleepForMicrosecondsLinuxX64(microseconds);
            else if (Architektur == 2) SleepForMicrosecondsLinuxARM64(microseconds);
            else SleepAusweichfunktion(microseconds);
        }

        public static uint OptimiereSleepForMicroseconds(uint ZieldauerInUs, uint maximumIterationen = 100, double zulFehler = 0.01)
        {
            int iteration = 0;
            double adjustmentFactor = 1.0;
            double errorPercentage;

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
            if (!System.IO.Directory.Exists(pfad))
            {
                Logger.Debug($"Ordner {pfad} existiert nicht. Erstelle Ordner.");
                System.IO.Directory.CreateDirectory(pfad);
            } else {
                Logger.Debug($"Ordner {pfad} existiert bereits.");
            }
        }
    }
}