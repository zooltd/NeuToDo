using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Xamarin.Essentials;

namespace NeuToDo {
    public class Startup {
        public static App Init(Action<HostBuilderContext, IServiceCollection> nativeConfigureServices)
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("NeuToDo.appsettings.json");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(c =>
                {
                    c.AddCommandLine(new string[] { $"ContentRoot={FileSystem.AppDataDirectory}" });
                    c.AddJsonStream(stream);
                })
                .ConfigureServices((c, x) =>
                {
                    nativeConfigureServices(c, x);
                    ConfigureServices(x);
                })
                .ConfigureLogging(l => l.AddConsole(o =>
                {
                    o.DisableColors = true;
                }))
                .Build();

            App.ServiceProvider = host.Services;

            return App.ServiceProvider.GetService<App>();
        }

        public static void ConfigureServices(IServiceCollection services) {
            services.AddHttpClient("mooc", config => {
                config.BaseAddress = new Uri("https://www.icourse163.org/");
                config.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
            }).ConfigurePrimaryHttpMessageHandler((() =>
                new HttpClientHandler() {
                    AllowAutoRedirect = true,
                    UseCookies = true,
                    CookieContainer = new CookieContainer()
                }));
        }
    }
}