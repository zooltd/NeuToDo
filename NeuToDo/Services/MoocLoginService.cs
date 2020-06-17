using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class MoocLoginService : ILoginService
    {
        private readonly IEventModelStorage<MoocEvent> _eventModelStorage;

        public MoocLoginService(IEventModelStorage<MoocEvent> eventModelStorage)
        {
            _eventModelStorage = eventModelStorage;
        }

        public async Task<bool> LoginAndFetchDataAsync(string userId, string password)
        {
            var getter = Startup.ServiceProvider.GetService<MoocInfoGetter>();
            try
            {
                await getter.WebCrawler(userId, password);
                await _eventModelStorage.ClearTableAsync();
                await _eventModelStorage.InsertAllAsync(MoocInfoGetter.EventList);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}