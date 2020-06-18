using System;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class LoginAndFetchDataService : ILoginAndFetchDataService
    {
        private readonly IEventModelStorageProvider _storageProvider;

        public LoginAndFetchDataService(IEventModelStorageProvider eventModelStorageProvider)
        {
            _storageProvider = eventModelStorageProvider;
        }

        public event EventHandler GetData;

        public async Task<bool> LoginAndFetchDataAsync(ServerType serverType, string userId, string password)
        {
            switch (serverType)
            {
                case ServerType.Neu:
                    var getter = new NeuSyllabusGetter(userId, password);
                    var neuStorage = await _storageProvider.GetEventModelStorage<NeuEvent>();
                    try
                    {
                        await getter.WebCrawler();
                        await neuStorage.ClearTableAsync();
                        await neuStorage.InsertAllAsync(NeuSyllabusGetter.EventList);
                        OnGetData();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                case ServerType.Mooc:
                    break;
                case ServerType.Bb:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serverType), serverType, null);
            }

            return false;
        }

        protected virtual void OnGetData()
        {
            GetData?.Invoke(this, EventArgs.Empty);
        }
    }
}