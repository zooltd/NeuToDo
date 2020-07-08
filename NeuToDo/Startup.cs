﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeuToDo.Services;
using System;
using System.Net;
using System.Net.Http;
using Xamarin.Essentials;

namespace NeuToDo
{
    public class Startup
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static CookieContainer CookieContainer = new CookieContainer();

        public static void Init()
        {
            var host = new HostBuilder().ConfigureHostConfiguration(c =>
            {
                c.AddCommandLine(new string[] {
                    $"ContentRoot={FileSystem.AppDataDirectory}"
                });
            }).ConfigureServices(Startup.ConfigureServices).Build();

            ServiceProvider = host.Services;
        }

        public static void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("mooc", config =>
            {
                config.BaseAddress = new Uri("https://www.icourse163.org/");
                config.DefaultRequestHeaders.Add("User-Agent",
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.61 Safari/537.36");
            }).ConfigurePrimaryHttpMessageHandler((() =>
                new HttpClientHandler() { AllowAutoRedirect = true }));
            services.AddHttpClient("neuInit", config => { })
                .ConfigurePrimaryHttpMessageHandler((() =>
                    new HttpClientHandler()
                    {
                        AllowAutoRedirect = false,
                        UseCookies = true,
                        CookieContainer = CookieContainer
                    }));
            services.AddHttpClient("neuReallocate", config => { })
                .ConfigurePrimaryHttpMessageHandler((() =>
                    new HttpClientHandler()
                    {
                        AllowAutoRedirect = true,
                        UseCookies = true,
                        CookieContainer = CookieContainer
                    }));

            services.AddTransient<NeuSyllabusGetter>();
            services.AddTransient<MoocInfoGetter>();
        }
    }
}