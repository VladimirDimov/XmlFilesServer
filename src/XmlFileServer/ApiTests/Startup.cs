using ApiTests.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ApiTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

            var configuration = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();


            services.AddScoped<ApiClient>();
            services.AddScoped<FormFileHelper>();

            services.AddHttpClient("XmlFileServer", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7123/");
            });
        }
    }
}
