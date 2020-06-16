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
using NeuToDo.Services;
using Xamarin.Essentials;

namespace NeuToDo {
    public class Startup {
        public static IServiceProvider ServiceProvider { get; set; }
        public static CookieContainer CookieContainer = new CookieContainer();

        public static void Init()
        {
            var a = Assembly.GetExecutingAssembly();
            using var stream = a.GetManifestResourceStream("appsettings.json");

            var host = new HostBuilder()
                .ConfigureServices(ConfigureServices)
                .Build();

            ServiceProvider = host.Services;
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
            services.AddHttpClient("neu1", config => { })
                .ConfigurePrimaryHttpMessageHandler((() =>
                    new HttpClientHandler() {
                        AllowAutoRedirect = false,
                        UseCookies = true,
                        CookieContainer = CookieContainer
                    }));
            services.AddHttpClient("neu2", config => { })
                .ConfigurePrimaryHttpMessageHandler((() =>
                    new HttpClientHandler()
                    {
                        AllowAutoRedirect = true,
                        UseCookies = true,
                        CookieContainer = CookieContainer
                    }));
            services.AddHttpClient();

            services.AddSingleton<NeuSyllabusGetter>();
        }
    }
}