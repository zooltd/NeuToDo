using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IEventModelStorageProvider
    {
        IEventModelStorage<NeuEventModel> GetNeuEventModelStorage();
    }
}