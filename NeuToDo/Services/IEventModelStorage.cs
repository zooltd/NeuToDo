using NeuToDo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IEventModelStorage<T> where T : EventModel
    {
        Task InsertAsync(T t);

        Task InsertAllAsync(IEnumerable<T> eventList);

        Task ClearTableAsync();

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllAsync(string code);

        Task MergeAsync(IList<T> eventList);
    }
}