using System;
using NeuToDo.Models;
using SQLite;
using System.Collections.Generic;
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
        {
            await _connection.InsertAsync(t);
        }

        public async Task InsertAllAsync(IEnumerable<T> eventList)
        {
            await _connection.InsertAllAsync(eventList);
        }

        public async Task ClearTableAsync()
        {
            await _connection.DeleteAllAsync<T>();
        }

        public async Task<List<T>> GetAllAsync() =>
            await _connection.Table<T>().ToListAsync();

        public async Task<List<T>> GetAllAsync(string code)
        {
            return await _connection.Table<T>().Where(e => (e.Code == code)).ToListAsync();
        }
    }
}