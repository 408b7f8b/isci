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
            api.Controllers.Applications.target_path = System.Environment.GetEnvironmentVariable("Applications");
            api.Controllers.Datamodels.target_path = System.Environment.GetEnvironmentVariable("Datamodels");
            api.Controllers.Eventmodels.target_path = System.Environment.GetEnvironmentVariable("Eventmodels");
            api.Controllers.Functionmodels.target_path = System.Environment.GetEnvironmentVariable("Functionmodels");
            api.Controllers.Services.target_path = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile)}/.config/systemd/user/";
            
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
