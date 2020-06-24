using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class LoginAndFetchDataService : ILoginAndFetchDataService
    {
        private readonly IEventModelStorageProvider _storageProvider;
        private static IHttpClientFactory _httpClientFactory;

        public LoginAndFetchDataService(IEventModelStorageProvider eventModelStorageProvider, IHttpClientFactory httpClientFactory)
        {
            _storageProvider = eventModelStorageProvider;
            _httpClientFactory = httpClientFactory;
        }

        // public event EventHandler GetData;

        public async Task<bool> LoginAndFetchDataAsync(ServerType serverType, string userId, string password)
        {
            switch (serverType)
            {
                case ServerType.Neu:
                    // var neuGetter = Startup.ServiceProvider.GetService<NeuSyllabusGetter>();
                    var neuGetter = new NeuSyllabusGetter(_httpClientFactory);
                    var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
                    try
                    {
                        await neuGetter.WebCrawler(userId, password);
                        await neuStorage.ClearTableAsync();
                        await neuStorage.InsertAllAsync(NeuSyllabusGetter.EventList);
                        _storageProvider.OnUpdateData();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                case ServerType.Mooc:
                    // var moocGetter = Startup.ServiceProvider.GetService<MoocInfoGetter>();
                    var moocGetter = new MoocInfoGetter(_httpClientFactory);
                    var moocStorage = await _storageProvider.GetEventModelStorage<MoocEvent>();
                    try
                    {
                        await moocGetter.WebCrawler(userId, password);
                        // await moocStorage.ClearTableAsync();
                        // await moocStorage.InsertAllAsync(MoocInfoGetter.EventList);
                        
                        _storageProvider.OnUpdateData();
                        return true;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return false;
                    }
                case ServerType.Bb:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }

            return false;
        }
    }
}