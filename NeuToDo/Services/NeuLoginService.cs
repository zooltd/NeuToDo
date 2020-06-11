using System;
using System.Linq;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class NeuLoginService : ILoginService
    {
        private readonly IEventModelStorage<NeuEventModel> _eventModelStorage;

        public NeuLoginService(IEventModelStorageProvider eventModelStorageProvider)
        {
            var task = eventModelStorageProvider.GetDatabaseConnection<NeuEventModel>();
            task.Wait();
            _eventModelStorage = task.Result;
        }

        public event EventHandler UpdateData;

        public async Task<bool> LoginAndFetchDataAsync(string userId, string password)
        {
            var getter = new NeuSyllabusGetter(userId, password);
            try
            {
                await getter.WebCrawler();
                await _eventModelStorage.ClearTableAsync();
                await _eventModelStorage.InsertAllAsync(NeuSyllabusGetter.EventList);

                return true;
            }
            catch (Exception e)
            {
                return false;
                // ignored
            }
        }

        protected virtual void OnUpdateData()
        {
            UpdateData?.Invoke(this, EventArgs.Empty);
        }
    }
}