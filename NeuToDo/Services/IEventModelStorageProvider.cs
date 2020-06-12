using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IEventModelStorageProvider
    {
        Task<IEventModelStorage<T>> GetEventModelStorage<T>()
            where T : EventModel, new();
    }
}