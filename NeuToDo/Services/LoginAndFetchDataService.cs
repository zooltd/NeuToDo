using GalaSoft.MvvmLight.Ioc;
using NeuToDo.Models;
using System;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class LoginAndFetchDataService : ILoginAndFetchDataService
    {
        private readonly IEventModelStorage<NeuEvent> _neuStorage;
        private readonly ISemesterStorage _semesterStorage;
        private readonly IDbStorageProvider _dbStorageProvider;

        public LoginAndFetchDataService(IDbStorageProvider dbStorageProvider)
        {
            _neuStorage = dbStorageProvider.GetEventModelStorage<NeuEvent>();
            _semesterStorage = dbStorageProvider.GetSemesterStorage();
            _dbStorageProvider = dbStorageProvider;
        }

        // public event EventHandler GetData;

        public async Task<bool> LoginAndFetchDataAsync(ServerType serverType, string userId, string password)
        {
            switch (serverType)
            {
                case ServerType.Neu:
                    // var neuGetter = Startup.ServiceProvider.GetService<NeuSyllabusGetter>();
                    // var neuGetter = new NeuSyllabusGetter(_httpClientFactory);
                    //TODO 依赖注入？
                    SimpleIoc.Default.Register<NeuSyllabusGetter>();
                    var neuGetter = SimpleIoc.Default.GetInstance<NeuSyllabusGetter>();

                    try
                    {
                        var (semester, neuCourses) = await neuGetter.LoginAndFetchData(userId, password);
                        // await neuStorage.ClearTableAsync();
                        // await neuStorage.InsertAllAsync(NeuSyllabusGetter.EventList);
                        await _semesterStorage.InsertOrReplaceAsync(semester);
                        await _neuStorage.MergeAsync(neuCourses);
                        _dbStorageProvider.OnUpdateData();
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
                    // var moocStorage = await _dbStorageProvider.GetEventModelStorage<MoocEvent>();
                    try
                    {
                        await moocGetter.WebCrawler(userId, password);
                        // await moocStorage.ClearTableAsync();
                        // await moocStorage.InsertAllAsync(MoocInfoGetter.EventList);
                        //TODO 重构 _dbStorageProvider.OnUpdateData()在LoginViewModel中
                        // _dbStorageProvider.OnUpdateData();
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