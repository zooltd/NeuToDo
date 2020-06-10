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
        private SQLiteAsyncConnection _connection;

        private SQLiteAsyncConnection Connection => _connection ?? (_connection = new SQLiteAsyncConnection(DbPath));

        private const string DbName = "events.sqlite3";

        public static readonly string DbPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);

        public async Task CreateTableAsync()
        {
            await Connection.CreateTableAsync<T>();
        }

        public async Task InsertAsync(T t)
        {
            await _connection.InsertAsync(t);
        }

        public async Task InsertAllAsync(IList<T> eventList)
        {
            await Connection.InsertAllAsync(eventList);
        }

        public async Task ClearTableAsync()
        {
            await Connection.DeleteAllAsync<T>();
        }

        public async Task<IList<T>> GetAllAsync() => await Connection.Table<T>().ToListAsync();

        public async Task CloseAsync() => await Connection.CloseAsync();
    }
}