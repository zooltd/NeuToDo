using NeuToDo.Models;

namespace NeuToDo.Services
{
    //TODO 存在用意
    public class EventModelStorageProvider : IEventModelStorageProvider
    {
        public IEventModelStorage<NeuEventModel> GetNeuEventModelStorage() => new EventModelStorage<NeuEventModel>();
    }
}