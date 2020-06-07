using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NeuToDo.Models;
using SQLite;

namespace NeuToDo.Services
{
    public class CourseEventStorage : IEventStorage
    {
        private SQLiteAsyncConnection _connection = null;

        private const string DbName = "events.sqlite3";

        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);


        public async Task CreateDatabase()
        {
            _connection = new SQLiteAsyncConnection(DbPath);
            await _connection.CreateTableAsync<CourseEventModel>();
        }


        public async Task ClearDatabase()
        {
            await _connection.DeleteAllAsync<CourseEventModel>();
        }

        public async Task Insert(EventModel e)
        {
            await _connection.InsertAsync(e);
        }

        public async Task InsertAll(IList<EventModel> eventList)
        {
            await _connection.InsertAllAsync(eventList);
        }

        public bool IsInitialized()
        {
            throw new NotImplementedException();
        }
    }
}