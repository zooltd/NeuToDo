using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class LoginAndFetchDataService : ILoginAndFetchDataService
    {
        private readonly IStorageProvider _storageProvider;
        // private static IHttpClientFactory _httpClientFactory;

        public LoginAndFetchDataService(IStorageProvider storageProvider)
        {
            _storageProvider = storageProvider;
            // _httpClientFactory = httpClientFactory;
        }

        // public event EventHandler GetData;

        public async Task<bool> LoginAndFetchDataAsync(ServerType serverType, string userId, string password)
        {
            switch (serverType)
            {
                case ServerType.Neu:
                    // var neuGetter = Startup.ServiceProvider.GetService<NeuSyllabusGetter>();
                    // var neuGetter = new NeuSyllabusGetter(_httpClientFactory);
                    SimpleIoc.Default.Register<NeuSyllabusGetter>();
                    var neuGetter = SimpleIoc.Default.GetInstance<NeuSyllabusGetter>();
                    var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
                    try
                    {
                        var neuEventList = await neuGetter.WebCrawler(userId, password);
                        // await neuStorage.ClearTableAsync();
                        // await neuStorage.InsertAllAsync(NeuSyllabusGetter.EventList);
                        await neuStorage.MergeAsync(neuEventList); //TODO Clear后InsertAll报错 ①排查 ②转换为原子性操作
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
                    // var moocGetter = new MoocInfoGetter(_httpClientFactory);
                    SimpleIoc.Default.Register<MoocInfoGetter>();
                    var moocGetter = SimpleIoc.Default.GetInstance<MoocInfoGetter>();
                    // var moocStorage = await _storageProvider.GetEventModelStorage<MoocEvent>();
                    try
                    {
                        await moocGetter.WebCrawler(userId, password);
                        // await moocStorage.ClearTableAsync();
                        // await moocStorage.InsertAllAsync(MoocInfoGetter.EventList);
                        //TODO 重构 _storageProvider.OnUpdateData()在LoginViewModel中
                        // _storageProvider.OnUpdateData();
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