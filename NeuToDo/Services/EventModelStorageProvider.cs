using NeuToDo.Models;

namespace NeuToDo.Services
{
    public class EventModelStorageProvider : IEventModelStorageProvider
    {
        public IEventModelStorage<NeuEventModel> GetNeuEventModelStorage() => new NewEventModelStorage<NeuEventModel>();
    }
}