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
        public static string Ressource, Datenstrukturen, Dateien;
        public static void Main(string[] args)
        {
            Ressource = "TEST";// System.Environment.GetEnvironmentVariable("ARCHITEKTUR_RESSOURCE");
            Datenstrukturen = System.Environment.GetEnvironmentVariable("ARCHITEKTUR_DATENSTRUKTUREN");
            api.Controllers.Applications.target_path = System.IO.Directory.GetCurrentDirectory(); //System.Environment.GetEnvironmentVariable("ARCHITEKTUR_ANWENDUNGEN");
            api.Controllers.Services.target_path = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";
            
            var handler = new System.Net.Http.HttpClientHandler();

            handler.ServerCertificateCustomValidationCallback += 
                (sender, certificate, chain, errors) =>
                {
                    return true;
                };

            api.Controllers.Applications.client = new System.Net.Http.HttpClient(handler);

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
