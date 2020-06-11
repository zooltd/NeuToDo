using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using SQLite;

namespace NeuToDo.Services
{ //TODO 单例
    public class EventModelStorage<T> : IEventModelStorage<T> where T : EventModel, new()
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

        public async Task<IList<T>> GetAllAsync() => await _connection.Table<T>().ToListAsync();

        public async Task CloseAsync() => await _connection.CloseAsync();
    }
}