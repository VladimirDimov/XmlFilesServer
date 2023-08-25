using ApiTests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ApiTests
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            AddConfigFile(services);

            services.AddScoped<ApiClient>();
            services.AddScoped<FormFileHelper>();
            services.AddSingleton<TestFilesHelper>();
            services.AddSingleton<DirectoryHelper>();

            services.AddHttpClient("XmlFileServer", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7123/");
            });
        }

        private static void AddConfigFile(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", true, true)
                                .Build();

            var settings = new TestAppSettings();
            configuration.GetSection("appSettings").Bind(settings);

            services.AddSingleton(settings);
        }
    }
}
