using NeuToDo.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NeuToDo.Services
{
    public class NeuLoginService : ILoginService
    {
        private readonly IEventModelStorage<NeuEvent> _eventModelStorage;

        public NeuLoginService(IEventModelStorage<NeuEvent> eventModelStorage)
        {
            _eventModelStorage = eventModelStorage;
        }

        public event EventHandler UpdateData;

        public async Task<bool> LoginAndFetchDataAsync(string userId, string password)
        {
            var getter = Startup.ServiceProvider.GetService<NeuSyllabusGetter>();
            try
            {
                await getter.WebCrawler(userId, password);
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