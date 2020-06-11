using NeuToDo.Models;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IEventModelStorageProvider
    {
        Task<IEventModelStorage<T>> GetDatabaseConnection<T>()
            where T : EventModel, new();
    }
}