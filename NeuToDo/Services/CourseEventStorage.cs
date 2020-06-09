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

        public SQLiteAsyncConnection Connection => _connection ?? (_connection = new SQLiteAsyncConnection(DbPath));

        private const string DbName = "events.sqlite3";

        private const string TableName = "courses";

        public static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DbName);


        public async Task CreateDatabase()
        {
            await Connection.CreateTableAsync<EventModel>();
        }

        public async Task ClearDatabase()
        {
            await Connection.DeleteAllAsync<EventModel>();
        }

        public async Task Insert(EventModel e)
        {
            await Connection.InsertAsync(e);
        }

        public async Task InsertAll(IList<EventModel> eventList)
        {
            await Connection.InsertAllAsync(eventList);
        }

        public async Task<List<EventModel>> GetAll() => await Connection.Table<EventModel>().ToListAsync();

        /// <summary>
        /// TODO Table Exist?
        /// </summary>
        /// <returns></returns>
        public bool DbExist()
        {
            // var tableExistsQuery = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{TableName}';";

            return File.Exists(DbPath);
        }
    }
}