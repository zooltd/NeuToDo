using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services {
    public interface IEventModelStorageProvider {
        Task<IEventModelStorage<T>> GetDatabaseConnection<T>()
            where T : EventModel, new();
    }
}