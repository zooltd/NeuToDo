using System.Collections.Generic;
using System.Threading.Tasks;
using NeuToDo.Models;

namespace NeuToDo.Services
{
    public interface IEventModelStorage<T> where T : EventModel
    {
        Task CreateTableAsync();

        Task InsertAsync(T t);

        Task InsertAllAsync(IList<T> eventList);

        Task ClearTableAsync();

        Task<IList<T>> GetAllAsync();
    }
}