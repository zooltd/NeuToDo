using System.Threading.Tasks;
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
            throw new System.NotImplementedException();
        }
    }
}