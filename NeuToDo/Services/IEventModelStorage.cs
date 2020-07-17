using NeuToDo.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public interface IEventModelStorage<T> where T : EventModel
    {
        Task InsertAsync(T t);

        Task InsertOrReplaceAsync(T t);

        Task InsertAllAsync(IEnumerable<T> eventList);

        Task UpdateAsync(T t);

        Task UpdateAllAsync(IEnumerable<T> eventList);

        Task DeleteAllAsync();

        Task<List<T>> GetAllAsync();

        Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predExpr);

        Task<bool> ExistAsync(Expression<Func<T, bool>> predExpr);
    }
}