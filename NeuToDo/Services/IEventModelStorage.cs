using System;
using NeuToDo.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IEventModelStorage<T> where T : EventModel
    {
        Task InsertAsync(T t);

        Task InsertAllAsync(IEnumerable<T> eventList);

        Task DeleteAllAsync(Expression<Func<T, bool>> predExpr);

        Task ClearTableAsync();

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllAsync(string code);

        Task MergeAsync(IList<T> eventList);
    }
}