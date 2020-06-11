using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IEventModelStorage<T> where T : EventModel
    {
        Task InsertAsync(T t);

        Task InsertAllAsync(IEnumerable<T> eventList);

        Task ClearTableAsync();

        Task<IList<T>> GetAllAsync();

        Task CloseAsync();
    }
}