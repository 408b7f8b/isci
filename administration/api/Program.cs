using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            library.Helfer.OrdnerPruefenErstellen("Abbilder");
            library.Helfer.OrdnerPruefenErstellen("Konfigurationen");
            library.Helfer.OrdnerPruefenErstellen("Anwendungen");
            library.Helfer.OrdnerPruefenErstellen("Beschreibungen");
            library.Helfer.OrdnerPruefenErstellen("Anweisungen");
            library.Helfer.OrdnerPruefenErstellen("Dateien");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
