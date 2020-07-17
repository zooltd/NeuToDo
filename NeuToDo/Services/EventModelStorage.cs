using NeuToDo.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace NeuToDo.Services
{
    public class EventModelStorage<T> : IEventModelStorage<T>
        where T : EventModel, new()
    {
        private readonly SQLiteAsyncConnection _connection;

        public EventModelStorage(SQLiteAsyncConnection connection)
        {
            _connection = connection;
        }

        public async Task InsertAsync(T t)
            => await _connection.InsertAsync(t);

        public async Task InsertOrReplaceAsync(T t)
            => await _connection.InsertOrReplaceAsync(t);

        public async Task InsertAllAsync(IEnumerable<T> eventList)
            => await _connection.InsertAllAsync(eventList);

        public async Task UpdateAsync(T t)
            => await _connection.UpdateAsync(t);

        public async Task UpdateAllAsync(IEnumerable<T> eventList)
            => await _connection.UpdateAllAsync(eventList);

        public async Task DeleteAllAsync()
            => await _connection.DeleteAllAsync<T>();

        public async Task<List<T>> GetAllAsync()
            => await _connection.Table<T>().ToListAsync();

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> predExpr)
            => await _connection.Table<T>().Where(predExpr).ToListAsync();

        public async Task<bool> ExistAsync(Expression<Func<T, bool>> predExpr)
            => await _connection.Table<T>().Where(predExpr).CountAsync() > 0;
    }
}