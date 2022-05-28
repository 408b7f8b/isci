using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Diagnostics;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddMvc(
                config => { 
                    config.Filters.Add(typeof(CustomExceptionFilter));
                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseExceptionHandler(
            options => {
                options.Run(
                async context =>
                {
                context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                context.Response.ContentType = "text/html";
                var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                if (ex != null)
                {
                    var err = $"<h1>Error: {ex.Error.Message}</h1>{ex.Error.StackTrace }";
                    await context.Response.WriteAsync(err).ConfigureAwait(false);
                }
                });
            }
            );
        }

        public class CustomExceptionFilter : IExceptionFilter
        {
            public void OnException(ExceptionContext context)
            {
                HttpStatusCode status = HttpStatusCode.InternalServerError;
                String message = String.Empty;
        
                var exceptionType = context.Exception.GetType();
                if (exceptionType == typeof(UnauthorizedAccessException))
                {
                    message = "Unauthorized Access";
                    status = HttpStatusCode.Unauthorized;
                }
                else if (exceptionType == typeof(NotImplementedException))
                {
                    message = "A server error occurred.";
                    status = HttpStatusCode.NotImplemented;
                }
                else
                {
                    message = context.Exception.Message;
                    status = HttpStatusCode.NotFound;
                }
                context.ExceptionHandled=true;
        
                HttpResponse response = context.HttpContext.Response;
                response.StatusCode = (int)status;
                response.ContentType = "application/json";
                var err = message + " " + context.Exception.StackTrace;
                response.WriteAsync(err);
            }
        }
    }
}
