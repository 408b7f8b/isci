using System;
using System.Linq;
using Common.Logging.Factory;
using Serilog;
using Tmds.Linux;

namespace isci
{
    public static class Logger
    {
        public enum Stufe
        {
            VERBOSE = 0,
            ALLES = 0,
            DEBUG = 1,
            INFORMATION = 2,
            WARNING = 3,
            WARNUNG = 3,
            ERROR = 4,
            FEHLER = 4,
            FATAL = 5
        }

        public static void Initialisieren()
        {
            var config = new LoggerConfiguration();
            config.WriteTo.File("startup.log", rollOnFileSizeLimit: true, fileSizeLimitBytes: 1000000, retainedFileCountLimit: 3);
            config.WriteTo.Console();
            Log.Logger = config.CreateLogger();
            Log.Information("Logging mit Initialeinstellungen in Konsole und startup.log.");
        }

        public static void Konfigurieren(Allgemein.Parameter parameter)
        {
            var config = new LoggerConfiguration();
            if (parameter.LoggingInDateiAktiv) config.WriteTo.File(parameter.OrdnerLogs + "/" + parameter.Identifikation + ".log", rollOnFileSizeLimit: true, fileSizeLimitBytes: parameter.LoggingInDateiMaxDateiGroesseInMb*1000000, retainedFileCountLimit: 3);
            if (parameter.LoggingInKonsoleAktiv) config.WriteTo.Console();
            Log.Logger = config.CreateLogger();
            Log.Information("Logging mit ab jetzt mit Konfigurationseinstellungen.");
        }

        public static void Alles(string nachricht)
        {
            Log.Verbose(nachricht);
        }

        public static void Debug(string nachricht)
        {
            Log.Debug(nachricht);
        }

        public static void Information(string nachricht)
        {
            Log.Information(nachricht);
        }

        public static void Warnung(string nachricht)
        {
            Log.Warning(nachricht);
        }

        public static void Fehler(string nachricht)
        {
            Log.Error(nachricht);
        }

        public static void Fatal(string nachricht)
        {
            Log.Fatal(nachricht);
        }
    }
}